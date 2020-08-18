using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.Civ.TimePeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class StorageFactory : IOnTimedUpdate, IOnTryChangeBlock, IAfterItemTypesDefinedExtender
    {
        public double NextUpdateTimeMin => 1;

        public double NextUpdateTimeMax => 4;

        public double NextUpdateTime { get; set; }
        public static Dictionary<string, int> MaxStackSize { get; set; } = new Dictionary<string, int>();

        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName { get; } = "IStorageUpgradeBlock";

        public Type ClassType { get; }

        public static Dictionary<string, IStorageUpgradeBlock> StorageBlockTypes { get; set; } = new Dictionary<string, IStorageUpgradeBlock>();

        public void OnTimedUpdate()
        {
            
        }

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony != null &&
                    (StorageBlockTypes.TryGetValue(tryChangeBlockData.TypeNew.Name, out var newBlock) ||
                    StorageBlockTypes.TryGetValue(tryChangeBlockData.TypeOld.Name, out var oldBlock)))
            {
                var pos = GetStockpilePosition(tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony);


            }
        }

        public static StockpilePosition GetStockpilePosition(Colony colony)
        {
            var sp = new StockpilePosition();
            var cs = ColonyState.GetColonyState(colony);

            if (cs.Positions.TryGetValue(StockpileBlock.Name, out var pos))
            {
                sp.Position = pos;
                var currentPeriod = PeriodFactory.GetTimePeriod(colony);
                sp.Min = pos.Add(StockpileBlock.StockpileSizes[currentPeriod].Item1);
                sp.Max = pos.Add(StockpileBlock.StockpileSizes[currentPeriod].Item2);
            }

            return sp;
        }

        public void AfterItemTypesDefined()
        {
            foreach (var s in LoadedAssembalies)
            {
                if (Activator.CreateInstance(s) is IStorageUpgradeBlock sb && !string.IsNullOrEmpty(sb.name))
                {
                    StorageBlockTypes[sb.name] = sb;
                }
            }
        }
    }
}
