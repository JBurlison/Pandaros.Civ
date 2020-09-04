using Pandaros.Civ.Storage;
using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Research
{
    public class StockpileSizeGoal : IResearchableCondition
    {
        public int StockpileSize { get; set; }

        public StockpileSizeGoal(int size)
        {
            StockpileSize = size;
        }

        public bool IsConditionMet(AbstractResearchable researchable, ColonyScienceState manager)
        {
            return StorageFactory.DefaultMax[manager.Colony] >= StockpileSize;
        }
    }
}
