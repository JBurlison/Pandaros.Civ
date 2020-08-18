using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.WorldGen;
using Pandaros.Civ.TimePeriods;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class StorageFactory : IOnTimedUpdate, IOnChangedBlock, IAfterItemTypesDefinedExtender, IAfterWorldLoad
    {
        public double NextUpdateTimeMin => 4;

        public double NextUpdateTimeMax => 7;

        public double NextUpdateTime { get; set; }
        public static Dictionary<Colony, Dictionary<ushort, int>> MaxStackSize { get; set; } = new Dictionary<Colony, Dictionary<ushort, int>>();
        public static Dictionary<Colony, int> DefaultMax = new Dictionary<Colony, int>();

        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName { get; } = "IStorageUpgradeBlock";

        public Type ClassType { get; }

        public static Dictionary<string, IStorageUpgradeBlock> StorageBlockTypes { get; set; } = new Dictionary<string, IStorageUpgradeBlock>();

        public void OnTimedUpdate()
        {
            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values)
            {
                if (MaxStackSize.TryGetValue(colony, out var maxStockpile))
                {
                    foreach(var itemId in colony.Stockpile.Items.Keys)
                    {
                        if (!maxStockpile.TryGetValue(itemId, out var max))
                        {
                            if (!DefaultMax.TryGetValue(colony, out max))
                                max = StorageBlockTypes[StockpileBlock.Name].GlobalUpgrade;
                        }

                        if (colony.Stockpile.Items[itemId] > max)
                            colony.Stockpile.Items[itemId] = max;
                    }
                }
                else
                {
                    foreach (var itemId in colony.Stockpile.Items.Keys)
                    {
                        var max = StorageBlockTypes[StockpileBlock.Name].GlobalUpgrade;

                        if (colony.Stockpile.Items[itemId] > max)
                            colony.Stockpile.Items[itemId] = max;
                    }
                }

                colony.Stockpile.SendToOwners();
            }
        }

        public void OnChangedBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            var colony = tryChangeBlockData?.RequestOrigin.AsPlayer?.ActiveColony;

            if (tryChangeBlockData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                colony != null &&
                    (StorageBlockTypes.ContainsKey(tryChangeBlockData.TypeNew.Name) ||
                    StorageBlockTypes.ContainsKey(tryChangeBlockData.TypeOld.Name)))
            {
                RecalcMax(colony);
            }
        }

        private static void RecalcMax(Colony colony)
        {
            var pos = GetStockpilePosition(colony);
            var blocksAroundStockpile = WorldHelper.GetBlocksInArea(pos.Min, pos.Max);
            int total = 0;
            Dictionary<string, int> byCategory = new Dictionary<string, int>();
            Dictionary<string, int> byType = new Dictionary<string, int>();

            foreach (var blockType in blocksAroundStockpile.Values)
            {
                if (StorageBlockTypes.TryGetValue(blockType.Name, out var storageUpgradeBlock))
                {
                    total += storageUpgradeBlock.GlobalUpgrade;

                    if (storageUpgradeBlock.CategoryUpgrades != null)
                        foreach (var kvp in storageUpgradeBlock.CategoryUpgrades)
                        {
                            if (!byCategory.ContainsKey(kvp.Key))
                                byCategory.Add(kvp.Key, 0);

                            byCategory[kvp.Key] = byCategory[kvp.Key] + kvp.Value;
                        }

                    if (storageUpgradeBlock.ItemUpgrades != null)
                        foreach (var kvp in storageUpgradeBlock.ItemUpgrades)
                        {
                            if (!byType.ContainsKey(kvp.Key))
                                byType.Add(kvp.Key, 0);

                            byType[kvp.Key] = byType[kvp.Key] + kvp.Value;
                        }
                }
            }

            if (!MaxStackSize.ContainsKey(colony))
                MaxStackSize[colony] = new Dictionary<ushort, int>();

            foreach (var item in ItemTypes._TypeByUShort.Values)
            {
                var totalStack = total;
                DefaultMax[colony] = total;

                if (item.Categories != null)
                {
                    foreach (var cat in item.Categories)
                    if (byCategory.TryGetValue(cat, out int catTotal))
                        totalStack += catTotal;
                }

                if (byType.TryGetValue(item.Name, out int itemTotal))
                    totalStack += itemTotal;

                MaxStackSize[colony][item.ItemIndex] = totalStack;
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

        public void AfterWorldLoad()
        {
            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values)
            {
                RecalcMax(colony);
            }
        }
    }
}
