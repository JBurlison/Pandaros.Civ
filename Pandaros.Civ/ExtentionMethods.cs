using Pandaros.Civ.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ
{
    public static class ExtentionMethods
    {
        public static List<StoredItem> ToStoredItemList(this List<InventoryItem> inventoryItems)
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

        public static void AddRange(this Dictionary<ushort, StoredItem> storedItem, List<InventoryItem> items, int stackCount = -1)
        {
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
    }
}
