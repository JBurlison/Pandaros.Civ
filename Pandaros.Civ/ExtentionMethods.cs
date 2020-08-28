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

        public static void AddRange(this List<StoredItem> storedItem, List<InventoryItem> items)
        {
            foreach (var item in items)
                storedItem.Add(item);
        }
    }
}
