using Pandaros.API.Models;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class CrateInventory
    {
        public Dictionary<ushort, StoredItem> Contents { get; set; } = new Dictionary<ushort, StoredItem>();
        public Dictionary<StorageType, List<StoredItem>> StorageTypeLookup = new Dictionary<StorageType, List<StoredItem>>();


        public ICrate CrateType { get; set; }

        public Vector3Int Position { get; set; }
        public Colony Colony { get; set; }
        public CrateInventory(ICrate crateType, Vector3Int position, Colony c)
        {
            CrateType = crateType;
            Position = position;
            Colony = c;

            StorageTypeLookup[StorageType.Stockpile] = new List<StoredItem>();
            StorageTypeLookup[StorageType.Crate] = new List<StoredItem>();
        }

        /// <summary>
        ///     Takes items from the crate inventory
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Returns number of items NOT able to taken.</returns>
        public Dictionary<ushort, StoredItem> TryTake(params StoredItem[] items)
        {
            Dictionary<ushort, StoredItem> retval = new Dictionary<ushort, StoredItem>();

            foreach (var item in items)
            {
                if (Contents.TryGetValue(item.Id, out var storedItem))
                {
                    var remaining = storedItem.RemoveReturnNotTaken(item);

                    if (remaining > 0)
                    {
                        retval[item.Id] = new StoredItem(item.Id, remaining);
                    }
                }

                if (Contents[item.Id] == 0)
                {
                    Contents.Remove(item.Id);
                    StorageTypeLookup[item.StorageType].Remove(item);
                    if (StorageFactory.ItemCrateLocations[Colony].TryGetValue(item.Id, out var posList))
                        posList.Remove(Position);
                }

               
            }

            return retval;
        }

        /// <summary>
        ///     Stores items in the crate
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Retuns the items that could not be stored.</returns>
        public List<StoredItem> TryAdd(params StoredItem[] items)
        {
            List<StoredItem> retval = new List<StoredItem>();

            foreach (var item in items)
            {
                if (Contents.TryGetValue(item.Id, out var storedItem))
                {
                    int cantStore = storedItem.Add(item);

                    if (cantStore > 0)
                    {
                        retval.Add(new StoredItem(item));
                    }
                }
                else
                {
                    if (Contents.Count >= CrateType.MaxNumberOfStacks)
                        retval.Add(new StoredItem(item));
                    else
                    {
                        if (item.Amount > CrateType.MaxCrateStackSize)
                        {
                            var remainder = item.Amount - CrateType.MaxCrateStackSize;
                            Contents[item.Id] = new StoredItem(item.Id, CrateType.MaxCrateStackSize, CrateType.MaxCrateStackSize);
                            item.Amount = remainder;
                            retval.Add(new StoredItem(item));
                        }
                        else
                        {
                            Contents[item.Id] = new StoredItem(item.Id, item, CrateType.MaxCrateStackSize);
                        }

                        StorageTypeLookup[item.StorageType].Add(Contents[item.Id]);
                    }
                }

                if (!StorageFactory.ItemCrateLocations[Colony].ContainsKey(item.Id))
                    StorageFactory.ItemCrateLocations[Colony].Add(item.Id, new List<Pipliz.Vector3Int>());

                var listRef = StorageFactory.ItemCrateLocations[Colony][item.Id];

                if (!listRef.Contains(Position))
                    listRef.Add(Position);
            }

            return retval;
        }
    }
}
