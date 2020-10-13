using BlockTypes;
using colonyserver.Assets.UIGeneration;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.Items;
using Newtonsoft.Json;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.localization;
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
    public class StorageFactory : IOnTimedUpdate, IOnChangedBlock, IAfterItemTypesDefinedExtender, IAfterWorldLoad, IOnLoadingImages
    {
        public int NextUpdateTimeMinMs => 2000;

        public int NextUpdateTimeMaxMs => 5000;
        public static CivCrateTracker CrateTracker { get; set; }
        public ServerTimeStamp NextUpdateTime { get; set; }
        public static Dictionary<Colony, Dictionary<ushort, int>> StockpileMaxStackSize { get; set; } = new Dictionary<Colony, Dictionary<ushort, int>>();
        public static Dictionary<Colony, int> DefaultMax = new Dictionary<Colony, int>();
        public static Dictionary<string, IStorageUpgradeBlock> StorageBlockTypes { get; set; } = new Dictionary<string, IStorageUpgradeBlock>();
        public static Dictionary<string, ICrate> CrateTypes { get; set; } = new Dictionary<string, ICrate>();
        public static LocalizationHelper LocalizationHelper { get; set; } = new LocalizationHelper(GameSetup.NAMESPACE, "Storage");
        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName { get; } = nameof(IStorageUpgradeBlock);

        public Type ClassType { get; }

        static bool _worldLoaded = false;

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnConstructTooltipUI, GameSetup.NAMESPACE + ".Storage.StorageFactory.ConstructTooltip")]
        static void ConstructTooltip(Players.Player player, ConstructTooltipUIData data)
        {
            ItemTypes.ItemType item = BuiltinBlocks.Types.air;

            if (data.hoverType == Shared.ETooltipHoverType.Item && !ItemTypes.TryGetType(data.hoverItem, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.PlayerRecipe && !ItemTypes.TryGetType(data.hoverItem, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.NPCRecipe && !ItemTypes.TryGetType(data.hoverItem, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.Science && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.ScienceCondition && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;
            else if (data.hoverType == Shared.ETooltipHoverType.ScienceUnlock && !ItemTypes.TryGetType(data.hoverKey, out item))
                return;

            if (item == BuiltinBlocks.Types.air)
                return;

            if (player.ActiveColony != null && StockpileMaxStackSize.TryGetValue(player.ActiveColony, out var itemDic) && itemDic.TryGetValue(item.ItemIndex, out var maxSize))
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Storage.MaxStackSize"))), 200),
                                                        (new Label(new LabelData(maxSize.ToString())), 60)
                                                    }));

            if (CrateTypes.TryGetValue(item.Name, out var crate))
            {
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Storage.MaxCrateStackSize"))), 200),
                                                        (new Label(new LabelData(crate.MaxCrateStackSize.ToString())), 60)
                                                    }));
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Storage.MaxNumberOfStacks"))), 200),
                                                        (new Label(new LabelData(crate.MaxNumberOfStacks.ToString())), 60)
                                                    }));
            }
            
            if (StorageBlockTypes.TryGetValue(item.Name, out var upgrade))
            {
                data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Storage.GlobalStorageUpgrade"))), 200),
                                                        (new Label(new LabelData(upgrade.GlobalStorageUpgrade.ToString())), 60)
                                                    }));


                if (upgrade.CategoryStorageUpgrades.Count > 0)
                {
                    data.menu.Items.Add(new EmptySpace(5));
                    data.menu.Items.Add(new Label(new LabelData(GameSetup.GetNamespace("Storage.CategoryStoreUpgrades"), ELabelAlignment.MiddleCenter, 18)));
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
                    data.menu.Items.Add(new Label(new LabelData(GameSetup.GetNamespace("Storage.ItemStoreUpgrades"), ELabelAlignment.MiddleCenter, 18)));
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

            foreach (var cKvp in CrateTracker.Positions.IterateTracker())
                cKvp.Inventory.CaclulateTimeouts();
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
        }

        public static Vector3Int GetClosestCrateLocation(Vector3Int pos, Colony colony)
        {
            if (!StorageFactory.CrateTracker.TryGetClosestInColony(colony, pos, out var ClosestCrate))
            {
                ClosestCrate = colony.Banners.FirstOrDefault().Position;

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

            float stacksTotal = 0;

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

                var count = colony.Stockpile.AmountContained(item.ItemIndex);

                if (count > 0)
                    stacksTotal += count;
            }

            var maxItems = colony.Stockpile.ItemCount * total;
            var fillPct = System.Math.Round(stacksTotal / maxItems, 2) * 100;

            foreach (var player in colony.Owners)
                if (player.ActiveColony == colony)
                { 
                    UIManager.AddorUpdateUIImage("ColonyStockpile" + colony.ColonyID, colonyshared.NetworkUI.UIGeneration.UIElementDisplayType.Colony, "StockpileBackground", new Vector3Int(135, -119, 0), colonyshared.NetworkUI.AnchorPresets.TopLeft, player);
                    UIManager.AddorUpdateUILabel("ColonyStockpile" + colony.ColonyID, colonyshared.NetworkUI.UIGeneration.UIElementDisplayType.Colony, LocalizationHelper.LocalizeOrDefault("StockpileSize", player, total.ToKMB(), fillPct.ToString()), new Vector3Int(137, -119, 0), colonyshared.NetworkUI.AnchorPresets.TopLeft, 270, player, 14);
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

        public void IOnLoadingImages(Dictionary<string, string> imagesToLoad)
        {
            imagesToLoad["StockpileBackground"] = GameSetup.Textures.GetPath(TextureType.image, "stockpilesize.png");
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

            if (ServerManager.BlockEntityCallbacks.TryGetAutoLoadedInstance<CivCrateTracker>(out var crateTracker))
                CrateTracker = crateTracker;

            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values.Where(c => c != null))
            {
                RecalcStockpileMaxSize(colony);
            }
        }
    }
}
