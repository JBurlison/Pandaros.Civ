using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using Pandaros.API.Models;

namespace Pandaros.Civ.Research
{
    public class ItemsInStockpileCondition : IResearchableCondition
    {
        public string ItemName { get; set; }
        public int Count { get; set; }
        private ItemId _itemId { get; set; }

        public ItemsInStockpileCondition(string itemName, int count)
        {
            ItemName = itemName;
            Count = count;
        }

        public bool IsConditionMet(AbstractResearchable researchable, ColonyScienceState manager)
        {
            if (_itemId == null)
                _itemId = ItemId.GetItemId(ItemName);

            return manager.Colony.Stockpile.Contains(_itemId, Count);
        }
    }
}
