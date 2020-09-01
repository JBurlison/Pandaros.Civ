using JetBrains.Annotations;
using Newtonsoft.Json;
using Pandaros.API;
using Pandaros.API.Models;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class StoredItem
    {
        public int MaxAmount { get; set; } = int.MaxValue;
        public int Amount { get; set; }
        public string Name
        {
            get
            {
                return Id.Name;
            }
            set
            {
                Id = ItemId.GetItemId(value);
            }
        }

        [JsonIgnore]
        public ItemId Id { get; set; } 
        public StorageType StorageType { get; set; }
        public ServerTimeStamp TTL { get; set; }

        public StoredItem() { }

        public StoredItem(ItemId id, int amount, int maxAmount = int.MaxValue, StorageType type = StorageType.Stockpile)
        {
            Id = id;
            Amount = amount;
            MaxAmount = maxAmount;
            StorageType = type;
            EnsureWithinMax(maxAmount);
            SetTTL(type);
        }

        public StoredItem(InventoryItem item, int maxAmount = int.MaxValue, StorageType type = StorageType.Stockpile)
        {
            Id = item.Type;
            Amount = item.Amount;
            StorageType = type;
            EnsureWithinMax(maxAmount);
            SetTTL(type);
        }

        public StoredItem(StoredItem item, int maxAmount = int.MaxValue, StorageType type = StorageType.Stockpile)
        {
            Id = item.Id;
            Amount = item.Amount;
            MaxAmount = item.MaxAmount;
            EnsureWithinMax(maxAmount);
            SetTTL(type);
        }

        private void SetTTL(StorageType type)
        {
            if (type == StorageType.Crate)
                TTL = ServerTimeStamp.Now.Add(Convert.ToInt64(TimeCycle.TotalDayLength.Value.TotalMilliseconds));
        }

        private void EnsureWithinMax(int maxAmount)
        {
            if (Amount > maxAmount)
                Amount = maxAmount;
        }

        /// <summary>
        ///     Removes items from the store
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Number of items taken.</returns>
        public int RemoveReturnTaken(int count)
        {
            if (count <= Amount)
            {
                Amount -= count;
                return count;
            }
            else
            {
                Amount = 0;
                return Amount;
            }
        }

        /// <summary>
        ///     Removes items from the store
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Number of items that could not be taken.</returns>
        public int RemoveReturnNotTaken(int count)
        {
            if (count <= Amount)
            {
                Amount -= count;
                return Amount;
            }
            else
            {
                count -= Amount;
                Amount = 0;
                return count;
            }
        }

        /// <summary>
        ///     Adds items to the store
        /// </summary>
        /// <param name="count"></param>
        /// <returns>returns number of items that could not be stored.</returns>
        public int Add(int count)
        {
            var newCount = count + Amount;

            if (newCount <= MaxAmount)
            {
                Amount = newCount;
                return 0;
            }
            else
            {
                Amount = MaxAmount;
                return newCount - MaxAmount;
            }
        }

        public static implicit operator int(StoredItem itemId)
        {
            return itemId.Amount;
        }

        public static implicit operator InventoryItem(StoredItem itemId)
        {
            return new InventoryItem(itemId.Id.Id, itemId.Amount);
        }

        public static implicit operator StoredItem(InventoryItem itemId)
        {
            return new StoredItem(itemId);
        }
    }
}
