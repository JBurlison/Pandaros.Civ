using Jobs;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API;
using Pandaros.API.Extender;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class StockpikeToCrateGoal : IPandaNpcGoal, IOnTimedUpdate
    {
        public static List<Vector3Int> InProgress { get; set; } = new List<Vector3Int>();
        public static Dictionary<Vector3Int, Dictionary<ushort, StoredItem>> ItemsNeeded { get; set; } = new Dictionary<Vector3Int, Dictionary<ushort, StoredItem>>();
        public StockpikeToCrateGoal() { }
        public StockpikeToCrateGoal(IJob job)
        {
            Job = job;
            PorterJob = job as PandaGoalJob;
        }

        public PandaGoalJob PorterJob { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(StockpikeToCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(StockpikeToCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public List<Vector3Int> ClosestLocations { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Stockpile;
        public bool CrateFull { get; set; }

        public int NextUpdateTimeMinMs => 5000;

        public int NextUpdateTimeMaxMs => 7000;

        public ServerTimeStamp NextUpdateTime { get; set; }
        public Vector3Int ClosestCrate { get; set; }

        public Vector3Int GetCrateSearchPosition()
        {
            return PorterJob.OriginalPosition;
        }

        public Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                if (CurrentCratePosition == Vector3Int.invalidPos)
                {
                    WalkingTo = StorageType.Stockpile;
                    return PorterJob.OriginalPosition;
                }

                return CurrentCratePosition;
            }
            else
                return StorageFactory.GetStockpilePosition(Job.Owner).Position;
        }

        public void LeavingGoal()
        {
           
        }
        public virtual void SetAsGoal()
        {

        }

        public virtual void LeavingJob()
        {

        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            state.JobIsDone = true;

            if (WalkingTo == StorageType.Stockpile)
            {
                if (CrateFull)
                {
                    Job.NPC.Inventory.Dump(Job.Owner.Stockpile);
                    CrateFull = false;
                }

                WalkingTo = StorageType.Crate;
                var nexPos = Vector3Int.invalidPos;

                foreach (var location in ClosestLocations)
                    if (!LastCratePosition.Contains(location) &&
                        !InProgress.Contains(location) &&
                        StorageFactory.CrateLocations[Job.Owner].TryGetValue(location, out var inv) &&
                        !inv.IsAlmostFull &&
                        ItemsNeeded.TryGetValue(location, out var itemsNeeded))
                    {
                        nexPos = location;
                        InProgress.Add(location);

                        var addToInv = StorageFactory.TryTakeItems(Job.Owner, itemsNeeded.Values);
                        var leftovers = new List<StoredItem>();

                        foreach (var item in addToInv)
                            if (Job.NPC.Inventory.Full)
                                leftovers.Add(item);
                            else
                                Job.NPC.Inventory.Add(item);

                        StorageFactory.StoreItems(Job.Owner, leftovers);
                        break;
                    }

                CurrentCratePosition = nexPos;

                if (nexPos == Vector3Int.invalidPos)
                {
                    LastCratePosition.Clear();
                    state.SetCooldown(5);
                }
                else
                {
                    state.SetCooldown(1);
                    state.SetIndicator(new Shared.IndicatorState(5, ItemId.GetItemId(StockpileBlock.Name).Id));
                }
            }
            else
            {
                if (StorageFactory.CrateLocations[Job.Owner].TryGetValue(CurrentCratePosition, out var crateInventory))
                {
                    WalkingTo = StorageType.Stockpile;
                    var leftovers = crateInventory.TryAdd(Job.NPC.Inventory.Inventory.Select(ii => new StoredItem(ii, int.MaxValue, StorageType.Crate)).ToArray());
                    state.SetCooldown(1);
                    state.SetIndicator(new Shared.IndicatorState(5, ColonyBuiltIn.ItemTypes.CRATE.Id));
                    if (leftovers.Count > 0)
                    {
                        Job.NPC.Inventory.Inventory.Clear();

                        foreach (var item in leftovers)
                            Job.NPC.Inventory.Add(item);

                        CrateFull = true;
                    }
                }
                else
                {
                    CrateFull = true;
                    state.SetCooldown(1);
                    state.SetIndicator(new Shared.IndicatorState(5, ColonyBuiltIn.ItemTypes.CRATE.Id, false, false));
                }
            }

        }

        public void OnTimedUpdate()
        {
            if (PandaJobFactory.ActiveGoalsByType.TryGetValue(nameof(CrateToStockpikeGoal), out var goalList))
                foreach (var goal in goalList)
                {
                    UpdateCrateLocationsForPorter(goal);
                }

            if (PandaJobFactory.ActiveGoalsByType.TryGetValue(nameof(StockpikeToCrateGoal), out goalList))
                foreach (var goal in goalList)
                {
                    UpdateCrateLocationsForPorter(goal);
                }

            RecalculateItemsNeeded();
        }

        public static void RecalculateItemsNeeded()
        {
            ItemsNeeded.Clear();

            foreach (var colony in ServerManager.ColonyTracker.ColoniesByID.Values)
                if (colony != null && StorageFactory.CrateLocations.TryGetValue(colony, out var dict))
                    foreach (var crate in dict.Keys)
                        foreach (var request in StorageFactory.CrateRequests)
                        {
                            var needed = request.GetItemsNeeded(crate);

                            foreach (var need in needed)
                            {
                                if (!ItemsNeeded.TryGetValue(crate, out var items))
                                {
                                    items = new Dictionary<ushort, StoredItem>();
                                    ItemsNeeded[crate] = items;
                                }

                                if (items.TryGetValue(need.Key, out var storedItem))
                                    storedItem.Add(needed.Count);
                                else
                                    items[need.Key] = need.Value;
                            }
                        }
        }

        public static void UpdateCrateLocationsForPorter(IPandaNpcGoal goal)
        {
            if (goal != null &&
                goal.Job.Owner != null &&
                StorageFactory.CrateLocations.TryGetValue(goal.Job.Owner, out var crateLocations))
            {
                if (goal is CrateToStockpikeGoal cts)
                    cts.ClosestLocations = goal.GetCrateSearchPosition().SortClosestPositions(crateLocations.Keys);
                else if (goal is StockpikeToCrateGoal stc)
                    stc.ClosestLocations = goal.GetCrateSearchPosition().SortClosestPositions(crateLocations.Keys);
            }
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
