using Pandaros.API;
using Pandaros.API.Models;
using Pipliz;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class CrateInventory 
    {
        public Dictionary<StorageType, Dictionary<ushort, StoredItem>> StorageTypeLookup = new Dictionary<StorageType, Dictionary<ushort, StoredItem>>();

        public ICrate CrateType { get; set; }

        public Vector3Int Position { get; set; }
        public Colony Colony { get; set; }


        public CrateInventory(JSONNode node, Colony c)
        {
            Colony = c;

            if (node.TryGetAs(nameof(Position), out JSONNode pos))
                Position = (Vector3Int)pos;

            if (node.TryGetAs(nameof(CrateType), out string ctype) && StorageFactory.CrateTypes.TryGetValue(ctype, out var crate))
                CrateType = crate;

            StorageTypeLookup = node[nameof(StorageTypeLookup)].JsonDeerialize<Dictionary<StorageType, Dictionary<ushort, StoredItem>>>();
        }

        public CrateInventory(ICrate crateType, Vector3Int position, Colony c)
        {
            CrateType = crateType;
            Position = position;
            Colony = c;

            StorageTypeLookup[StorageType.Stockpile] = new Dictionary<ushort, StoredItem>();
            StorageTypeLookup[StorageType.Crate] = new Dictionary<ushort, StoredItem>();
        }

        public bool IsAlmostFull
        {
            get
            {
                if (StorageTypeLookup[StorageType.Stockpile].Count >= CrateType.MaxNumberOfStacks - 2)
                    return true;

                foreach (var item in StorageTypeLookup[StorageType.Stockpile].Values)
                    if (item.Amount >= item.MaxAmount / 2)
                        return true;

                return false;
            }
        }

        public Dictionary<ushort, StoredItem> GetAllItems()
        {
            var retVal = new Dictionary<ushort, StoredItem>();

            foreach (var storageTypeKvp in StorageTypeLookup)
                foreach(var item in storageTypeKvp.Value)
                    if (retVal.TryGetValue(item.Key, out var storedItem))
                    {
                        storedItem.MaxAmount += item.Value.MaxAmount;
                        storedItem.Add(item.Value);
                    }
                    else
                    {
                        retVal.Add(item.Key, new StoredItem(item.Value));
                    }

            return retVal;
        }

        public JSONNode ToJSON()
        {
            JSONNode  node = new JSONNode();
            node[nameof(Colony)] = new JSONNode(Colony.ColonyID);
            node[nameof(Position)] = (JSONNode)Position;
            node[nameof(CrateType)] = new JSONNode(CrateType.name);
            node[nameof(StorageTypeLookup)] = StorageTypeLookup.JsonSerialize();
            return node;
        }


        public void CaclulateTimeouts()
        {
            var now = ServerTimeStamp.Now;

            foreach (var item in StorageTypeLookup[StorageType.Crate].Values)
                if (item.StorageType == StorageType.Crate && item.TTL < now)
                {
                    item.StorageType = StorageType.Stockpile;
                    StorageTypeLookup[StorageType.Crate].Remove(item.Id);

                    if (StorageTypeLookup[StorageType.Stockpile].TryGetValue(item.Id, out var existingItem))
                        existingItem.Add(item.Amount);
                    else
                        StorageTypeLookup[StorageType.Stockpile][item.Id] =item;
                }
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
                foreach (var typeKvp in StorageTypeLookup)
                    if (typeKvp.Value.TryGetValue(item.Id, out var storedItem))
                    {
                        var remaining = storedItem.RemoveReturnNotTaken(item);

                        if (remaining > 0)
                        {
                            retval[item.Id] = new StoredItem(item.Id, remaining);
                        }

                        if (typeKvp.Value[item.Id] == 0)
                        {
                            typeKvp.Value.Remove(item.Id);

                            if (StorageFactory.ItemCrateLocations[Colony].TryGetValue(item.Id, out var posList))
                                posList.Remove(Position);
                        }

                        if (remaining == 0)
                            break;
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
                if (!StorageTypeLookup.TryGetValue(item.StorageType, out var storedItems))
                {
                    storedItems = new Dictionary<ushort, StoredItem>();
                    StorageTypeLookup[item.StorageType] = storedItems;
                }   
                
                if (storedItems.TryGetValue(item.Id, out var storedItem))
                {
                    int cantStore = storedItem.Add(item);

                    if (cantStore > 0)
                    {
                        retval.Add(new StoredItem(item));
                    }
                }
                else
                {
                    if (storedItems.Count >= CrateType.MaxNumberOfStacks)
                        retval.Add(new StoredItem(item, CrateType.MaxCrateStackSize));
                    else
                    {
                        if (item.Amount > CrateType.MaxCrateStackSize)
                        {
                            var remainder = item.Amount - CrateType.MaxCrateStackSize;
                            storedItems[item.Id] = new StoredItem(item.Id, CrateType.MaxCrateStackSize, CrateType.MaxCrateStackSize);
                            item.Amount = remainder;
                            retval.Add(new StoredItem(item));
                        }
                        else
                        {
                            storedItems[item.Id] = new StoredItem(item.Id, item, CrateType.MaxCrateStackSize);
                        }
                    }
                }

                if (!StorageFactory.ItemCrateLocations[Colony].ContainsKey(item.Id))
                    StorageFactory.ItemCrateLocations[Colony].Add(item.Id, new List<Vector3Int>());

                var listRef = StorageFactory.ItemCrateLocations[Colony][item.Id];

                if (!listRef.Contains(Position))
                    listRef.Add(Position);
            }

            return retval;
        }
    }
}
