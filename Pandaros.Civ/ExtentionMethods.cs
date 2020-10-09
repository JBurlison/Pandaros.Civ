using NPC;
using Pandaros.Civ.Storage;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ
{
    public static class ExtentionMethods
    {
        public static List<StoredItem> ToStoredItemList(this IEnumerable<InventoryItem> inventoryItems)
        {
            List<StoredItem> retval = new List<StoredItem>();

            foreach (var item in inventoryItems)
                retval.Add(new StoredItem(item));

            return retval;
        }

        public static StoredItem[] ToStoredItemArray(this List<InventoryItem> inventoryItems)
        {
            StoredItem[] retval = new StoredItem[inventoryItems.Count - 1];
            int i = 0;

            foreach (var item in inventoryItems)
            {
                retval[i] = new StoredItem(item);
                i++;
            }

            return retval;
        }

        public static void AddRange(this Dictionary<ushort, StoredItem> storedItem, IEnumerable<InventoryItem> items, int stackCount = -1)
        {
            if (items != null)
                foreach (var item in items)
                {
                    if (storedItem.TryGetValue(item.Type, out var exisiting))
                    {
                        if (stackCount < 0)
                            exisiting.Amount += item.Amount;
                        else
                            exisiting.Amount = stackCount;
                    }
                    else
                    {
                        if (stackCount < 0)
                            storedItem.Add(item.Type, item);
                        else
                            storedItem.Add(item.Type, new StoredItem(item.Type, stackCount));
                    }
                }
        }

        public static void AddRange(this Dictionary<ushort, StoredItem> storedItem, Dictionary<ushort, StoredItem> items, int stackCount = -1)
        {
            if (items != null)
                foreach (var item in items.Values)
                {
                    if (storedItem.TryGetValue(item.Id, out var exisiting))
                    {
                        if (stackCount < 0)
                            exisiting.Amount += item.Amount;
                        else
                            exisiting.Amount = stackCount;
                    }
                    else
                    {
                        if (stackCount < 0)
                            storedItem.Add(item.Id, item);
                        else
                            storedItem.Add(item.Id, new StoredItem(item.Id, stackCount));
                    }
                }
        }

        public static void Add(this NPCInventory inventory, IEnumerable<StoredItem> items)
        {
            if (items != null)
                foreach (var item in items)
                    inventory.Add(item);
        }

        public static void Add(this NPCInventory inventory, IEnumerable<InventoryItem> items, int multiplier)
        {
            if (items != null)
                foreach (var item in items)
                {
                    inventory.Add(item * multiplier);
                }
        }

        public static void Add(this Stockpile stockpile, IEnumerable<StoredItem> items)
        {
            if (items != null)
                foreach (var item in items)
                    stockpile.Add(item.Id, item.Amount);
        }

        public static void Add(this Inventory inventory, IEnumerable<StoredItem> items)
        {
            if (items != null)
                foreach (var item in items)
                    inventory.TryAdd(item.Id, item.Amount);
        }

        public static string ToKMB(this int num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
