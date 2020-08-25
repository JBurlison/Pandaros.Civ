using JetBrains.Annotations;
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
        public ItemId Id { get; set; }


        public StoredItem(ItemId id, int amount, int maxAmount = int.MaxValue)
        {
            Id = id;
            Amount = amount;
            MaxAmount = maxAmount;

            if (amount > maxAmount)
                Amount = maxAmount;
        }

        public StoredItem(StoredItem item)
        {
            Id = item.Id;
            Amount = item.Amount;
            MaxAmount = item.MaxAmount;
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
    }
}
