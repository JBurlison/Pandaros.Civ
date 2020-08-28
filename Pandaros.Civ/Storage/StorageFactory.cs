using ModLoaderInterfaces;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.Models;
using Pandaros.API.WorldGen;
using Pandaros.Civ.TimePeriods;
using Pipliz;
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
        public int NextUpdateTimeMinMs => 2000;

        public int NextUpdateTimeMaxMs => 5000;

        public ServerTimeStamp NextUpdateTime { get; set; }
        public static Dictionary<Colony, Dictionary<ushort, int>> StockpileMaxStackSize { get; set; } = new Dictionary<Colony, Dictionary<ushort, int>>();
        public static Dictionary<Colony, int> DefaultMax = new Dictionary<Colony, int>();
        public static Dictionary<Colony, Dictionary<Vector3Int, CrateInventory>> CrateLocations { get; set; } = new Dictionary<Colony, Dictionary<Vector3Int, CrateInventory>>();
        public static Dictionary<string, IStorageUpgradeBlock> StorageBlockTypes { get; set; } = new Dictionary<string, IStorageUpgradeBlock>();
        public static Dictionary<string, ICrate> CrateTypes { get; set; } = new Dictionary<string, ICrate>();
        public static Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>> ItemCrateLocations { get; set; } = new Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>>();

        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName { get; } = nameof(IStorageUpgradeBlock);

        public Type ClassType { get; }

        bool _worldLoaded = false;

        public static void StoreItems(Colony c, params StoredItem[] storedItems)
        {
            foreach (var item in storedItems)
                c.Stockpile.Add(item.Id, item.Amount);
        }
        
        public void OnTimedUpdate()
        {
            if (!_worldLoaded)
                return;
            
            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values)
            {
                if (StockpileMaxStackSize.TryGetValue(colony, out var maxStockpile))
                {
                    foreach(var itemId in colony.Stockpile.Items.Keys)
                    {
                        if (!maxStockpile.TryGetValue(itemId, out var max))
                        {
                            if (!DefaultMax.TryGetValue(colony, out max))
                                max = StorageBlockTypes[StockpileBlock.Name].GlobalStorageUpgrade;
                        }

                        if (colony.Stockpile.Items[itemId] > max)
                            colony.Stockpile.Items[itemId] = max;
                    }
                }
                else
                {
                    foreach (var itemId in colony.Stockpile.Items.Keys)
                    {
                        var max = StorageBlockTypes[StockpileBlock.Name].GlobalStorageUpgrade;

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
                return;
            }

            if (tryChangeBlockData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                colony != null)
                if (CrateTypes.TryGetValue(tryChangeBlockData.TypeOld.Name, out var oldCrate))
                {
                    CrateLocations[colony].Remove(tryChangeBlockData.Position);

                    foreach (var item in ItemCrateLocations[colony])
                        item.Value.Remove(tryChangeBlockData.Position);
                }
                else if (CrateTypes.TryGetValue(tryChangeBlockData.TypeNew.Name, out var newCrate))
                {
                    CrateLocations[colony].Add(tryChangeBlockData.Position, new CrateInventory(newCrate, tryChangeBlockData.Position, colony));
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
                    total += storageUpgradeBlock.GlobalStorageUpgrade;

                    if (storageUpgradeBlock.CategoryStorageUpgrades != null)
                        foreach (var kvp in storageUpgradeBlock.CategoryStorageUpgrades)
                        {
                            if (!byCategory.ContainsKey(kvp.Key))
                                byCategory.Add(kvp.Key, 0);

                            byCategory[kvp.Key] = byCategory[kvp.Key] + kvp.Value;
                        }

                    if (storageUpgradeBlock.ItemStorageUpgrades != null)
                        foreach (var kvp in storageUpgradeBlock.ItemStorageUpgrades)
                        {
                            if (!byType.ContainsKey(kvp.Key))
                                byType.Add(kvp.Key, 0);

                            byType[kvp.Key] = byType[kvp.Key] + kvp.Value;
                        }
                }
            }

            if (!StockpileMaxStackSize.ContainsKey(colony))
                StockpileMaxStackSize[colony] = new Dictionary<ushort, int>();

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

                StockpileMaxStackSize[colony][item.ItemIndex] = totalStack;
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
                if (!CrateLocations.ContainsKey(colony))
                    CrateLocations.Add(colony, new Dictionary<Vector3Int, CrateInventory>());

                if (!ItemCrateLocations.ContainsKey(colony))
                    ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());

                RecalcMax(colony);
            }

            _worldLoaded = true;
        }
    }
}
