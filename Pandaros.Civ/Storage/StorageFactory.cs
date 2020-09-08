using BlockTypes;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.Items;
using Newtonsoft.Json;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.Models;
using Pandaros.API.WorldGen;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.TimePeriods;
using Pipliz;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    [ModLoader.ModManager]
    public class StorageFactory : IOnTimedUpdate, IOnChangedBlock, IAfterItemTypesDefinedExtender, IAfterWorldLoad, IOnSavingColony, IOnLoadingColony
    {
        public int NextUpdateTimeMinMs => 2000;

        public int NextUpdateTimeMaxMs => 5000;

        public ServerTimeStamp NextUpdateTime { get; set; }
        public static Dictionary<Colony, Dictionary<ushort, int>> StockpileMaxStackSize { get; set; } = new Dictionary<Colony, Dictionary<ushort, int>>();
        public static Dictionary<Colony, int> DefaultMax = new Dictionary<Colony, int>();
        public static Dictionary<Colony, Dictionary<Vector3Int, CrateInventory>> CrateLocations { get; set; } = new Dictionary<Colony, Dictionary<Vector3Int, CrateInventory>>();
        public static Dictionary<string, IStorageUpgradeBlock> StorageBlockTypes { get; set; } = new Dictionary<string, IStorageUpgradeBlock>();
        public static Dictionary<string, ICrate> CrateTypes { get; set; } = new Dictionary<string, ICrate>();
        public static List<ICrateRequest> CrateRequests { get; set; } = new List<ICrateRequest>();
        public static Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>> ItemCrateLocations { get; set; } = new Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>>();

        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName { get; } = nameof(IStorageUpgradeBlock);

        public Type ClassType { get; }

        static bool _worldLoaded = false;

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnConstructTooltipUI, GameSetup.NAMESPACE + ".Storage.StorageFactory.ConstructTooltip")]
        static void ConstructTooltip(ConstructTooltipUIData data)
        {
            ItemTypes.ItemType item = BuiltinBlocks.Types.air;

            if (data.hoverType == Shared.ETooltipHoverType.Item && !ItemTypes.TryGetType(data.hoverItem, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.PlayerRecipe && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.NPCRecipe && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.Science && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.ScienceCondition && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.ScienceUnlock && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;

            if (item == BuiltinBlocks.Types.air)
                return;

            if (CrateTypes.TryGetValue(item.Name, out var crate))
            {
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("storage.MaxCrateStackSize"))), 200),
                                                        (new Label(new LabelData(crate.MaxCrateStackSize.ToString())), 60)
                                                    }));
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("storage.MaxNumberOfStacks"))), 200),
                                                        (new Label(new LabelData(crate.MaxNumberOfStacks.ToString())), 60)
                                                    }));
            }
            
            if (StorageBlockTypes.TryGetValue(item.Name, out var upgrade))
            {
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("storage.GlobalStorageUpgrade"))), 200),
                                                        (new Label(new LabelData(upgrade.GlobalStorageUpgrade.ToString())), 60)
                                                    }));


                if (upgrade.CategoryStorageUpgrades.Count > 0)
                {
                    data.menu.Items.Add(new EmptySpace(5));
                    data.menu.Items.Add(new Label(new LabelData(GameSetup.GetNamespace("storage.CategoryStoreUpgrades"), ELabelAlignment.MiddleCenter, 18)));
                    data.menu.Items.Add(new Line(UnityEngine.Color.white, 3));
                    foreach (var csu in upgrade.CategoryStorageUpgrades)
                        data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(csu.Key)), 200),
                                                        (new Label(new LabelData(csu.Value.ToString())), 60)
                                                    }));
                }

                if (upgrade.ItemStorageUpgrades.Count > 0)
                {
                    data.menu.Items.Add(new EmptySpace(5));
                    data.menu.Items.Add(new Label(new LabelData(GameSetup.GetNamespace("storage.ItemStoreUpgrades"), ELabelAlignment.MiddleCenter, 18)));
                    data.menu.Items.Add(new Line(UnityEngine.Color.white, 3));
                    foreach (var csu in upgrade.ItemStorageUpgrades)
                        data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(csu.Key, ELabelAlignment.Default, 16, LabelData.ELocalizationType.Type)), 200),
                                                        (new Label(new LabelData(csu.Value.ToString())), 60)
                                                    }));
                }
            }
        }

        public void OnSavingColony(Colony colony, JSONNode data)
        {
            if (CrateLocations.TryGetValue(colony, out var cl))
            {
                if (!data.HasChild(nameof(CrateLocations)))
                    data[nameof(CrateLocations)] = new JSONNode();

                var locationsNode = new JSONNode();

                foreach (var crateLocation in cl)
                    locationsNode[crateLocation.Key.ToString()] = crateLocation.Value.ToJSON();

                data[nameof(CrateLocations)] = locationsNode;
            }

            if (ItemCrateLocations.TryGetValue(colony, out var icl))
            {
                if (!data.HasChild(nameof(ItemCrateLocations)))
                    data[nameof(ItemCrateLocations)] = new JSONNode();

                var itemsLocs = new JSONNode();

                foreach(var kvp in icl)
                {
                    var locs = new JSONNode(NodeType.Array);

                    foreach (var l in kvp.Value)
                        locs.AddToArray((JSONNode)l);

                    itemsLocs[kvp.Key.ToString()] = locs;
                }

                data[nameof(ItemCrateLocations)] = itemsLocs;
            }
        }

        public void OnLoadingColony(Colony colony, JSONNode data)
        {
            if (data.TryGetAs<JSONNode>(nameof(CrateLocations), out var crateJson))
            {
                    if (!CrateLocations.ContainsKey(colony))
                        CrateLocations.Add(colony, new Dictionary<Vector3Int, CrateInventory>());

                    foreach (var loc in crateJson.LoopObject())
                        CrateLocations[colony][Vector3Int.Parse(loc.Key)] = new CrateInventory(loc.Value, colony);
            }

            if (data.TryGetAs<JSONNode>(nameof(ItemCrateLocations), out var icl))
            {
                if (!ItemCrateLocations.ContainsKey(colony))
                    ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());

                foreach (var kvp in icl.LoopObject())
                {
                    List<Vector3Int> locs = new List<Vector3Int>();

                    foreach (var l in kvp.Value.LoopArray())
                        locs.Add((Vector3Int)l);

                    ItemCrateLocations[colony][Convert.ToUInt16(kvp.Key)] = locs;
                }
            }
        }

        /// <summary>
        ///     Stores items and discards any items that could not fit.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="storedItems"></param>
        public static void StoreItems(Colony c, params StoredItem[] storedItems)
        {
            foreach (var item in storedItems)
                c.Stockpile.Add(item.Id, item.Amount);

            c.Stockpile.SendToOwners();
        }

        /// <summary>
        ///     Stores items and discards any items that could not fit.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="storedItems"></param>
        public static void StoreItems(Colony c, IEnumerable<StoredItem> storedItems)
        {
            foreach (var item in storedItems)
                c.Stockpile.Add(item.Id, item.Amount);
        }

        /// <summary>
        ///     Stores items and discards any items that could not fit.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="storedItems"></param>
        public static void StoreItems(Colony c, IEnumerable<InventoryItem> storedItems)
        {
            foreach (var item in storedItems)
                c.Stockpile.Add(item);
        }

        /// <summary>
        ///     Tries to take items, returns items that the stickpile has.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="storedItems"></param>
        /// <returns></returns>
        public static StoredItem[] TryTakeItems(Colony c, IEnumerable<StoredItem> storedItems)
        {
            List<StoredItem> retVal = new List<StoredItem>();

            foreach (var item in storedItems)
                if (c.Stockpile.Items.TryGetValue(item.Id, out var count))
                {
                    if (count >= item.Amount)
                    {
                        c.Stockpile.TryRemove(item.Id, item.Amount, false);
                        retVal.Add(new StoredItem(item));
                    }
                    else
                    {
                        retVal.Add(new StoredItem(item.Id, count));
                        c.Stockpile.TryRemove(item.Id, count, false);
                    }
                }

            c.Stockpile.SendToOwners();

            return retVal.ToArray();
        }

        /// <summary>
        ///     Tries to take items, returns items that it COULD NOT get.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="storedItems"></param>
        /// <returns></returns>
        public static StoredItem[] TryTakeItemsReturnRemaining(Colony c, params StoredItem[] storedItems)
        {
            List<StoredItem> retVal = new List<StoredItem>();

            foreach (var item in storedItems)
                if (c.Stockpile.Items.TryGetValue(item.Id, out var count))
                {
                    if (count >= item.Amount)
                        c.Stockpile.TryRemove(item.Id, item.Amount, false);
                    else
                    {
                        retVal.Add(new StoredItem(item.Id, item.Amount - count));
                        c.Stockpile.TryRemove(item.Id, count);
                    }
                }
                else
                {
                    retVal.Add(new StoredItem(item));
                }

            c.Stockpile.SendToOwners();

            return retVal.ToArray();
        }

        public void OnTimedUpdate()
        {
            if (!_worldLoaded && ChunkQueue.CompletedInitialLoad)
                return;

            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values.Where(c => c != null))
            {
                RecalcStockpileMaxSize(colony);

                if (StockpileMaxStackSize.TryGetValue(colony, out var maxStockpile))
                {
                    foreach (var itemId in colony.Stockpile.Items.Keys)
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

            foreach (var cKvp in CrateLocations.Values)
            {
                foreach (var inv in cKvp.Values)
                {
                    inv.CaclulateTimeouts();
                }
            }
        }

        public void OnChangedBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            var colony = tryChangeBlockData?.RequestOrigin.AsPlayer?.ActiveColony;

            if (colony == null)
                colony = tryChangeBlockData.RequestOrigin.AsColony;

            if (colony != null &&
                    (StorageBlockTypes.ContainsKey(tryChangeBlockData.TypeNew.Name) ||
                    StorageBlockTypes.ContainsKey(tryChangeBlockData.TypeOld.Name)))
            {
                RecalcStockpileMaxSize(colony);
                return;
            }

            if (colony != null && CrateTypes.TryGetValue(tryChangeBlockData.TypeOld.Name, out var oldCrate))
            {
                /// empty the crate. TODO may want to do something other than magically teleporting.
                if (CrateLocations[colony].TryGetValue(tryChangeBlockData.Position, out var inv))
                    StoreItems(colony, inv.GetAllItems().Values);

                CrateLocations[colony].Remove(tryChangeBlockData.Position);

                foreach (var item in ItemCrateLocations[colony])
                    item.Value.Remove(tryChangeBlockData.Position);
            }
            else if (CrateTypes.TryGetValue(tryChangeBlockData.TypeNew.Name, out var newCrate))
            {
                if (!CrateLocations.ContainsKey(colony))
                    CrateLocations.Add(colony, new Dictionary<Vector3Int, CrateInventory>());

                CrateLocations[colony][tryChangeBlockData.Position] = new CrateInventory(newCrate, tryChangeBlockData.Position, colony);
            }
        }

        public static Vector3Int GetClosestCrateLocation(Vector3Int pos, Colony colony)
        {
            var ClosestCrate = colony.Banners.FirstOrDefault().Position;

            if (StorageFactory.CrateLocations.TryGetValue(colony, out var crateLocs))
                ClosestCrate = pos.GetClosestPosition(crateLocs.Keys.ToList());
            else
            {
                var stockpileLoc = StorageFactory.GetStockpilePosition(colony);

                if (stockpileLoc.Position != default(Vector3Int))
                    ClosestCrate = stockpileLoc.Position;
            }

            return ClosestCrate;
        }

        public static void RecalcStockpileMaxSize(Colony colony)
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

            if (total == 0)
                total = StorageBlockTypes[StockpileBlock.Name].GlobalStorageUpgrade;

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
            _worldLoaded = true;

            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values.Where(c => c != null))
            {
                if (!CrateLocations.ContainsKey(colony))
                    CrateLocations.Add(colony, new Dictionary<Vector3Int, CrateInventory>());

                if (!ItemCrateLocations.ContainsKey(colony))
                    ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());

                RecalcStockpileMaxSize(colony);
            }
        }
    }
}
