using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.API;
using Jobs;

namespace Pandaros.Civ.Jobs.Goals
{
    public class GetItemsFromCrateGoal : IPandaNpcGoal
    {

        public GetItemsFromCrateGoal(IJob job, Vector3Int originalPos, IPandaNpcGoal nextGoal, StoredItem[] itemsToGet, IPandaNpcGoal itemsForGoal)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet;
            ItemsForGoal = itemsForGoal;
            OriginalPosition = originalPos;
        }

        public GetItemsFromCrateGoal(IJob job, Vector3Int originalPos, IPandaNpcGoal nextGoal, List<InventoryItem> itemsToGet, IPandaNpcGoal itemsForGoal)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet.Select(i => new StoredItem(i)).ToArray();
            ItemsForGoal = itemsForGoal;
            OriginalPosition = originalPos;
        }

        public Vector3Int OriginalPosition { get; set; }
        public Vector3Int ClosestCrate { get; set; }
        public StoredItem[] ItemsToGet { get; set; }
        public IPandaNpcGoal ItemsForGoal { get; set; }
        public IPandaNpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(GetItemsFromCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(GetItemsFromCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; }
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        float _waitTime = Pipliz.Random.NextFloat(.4f, .8f);

        public Vector3Int GetPosition()
        {
            List<Vector3Int> cratesWithItems = new List<Vector3Int>();
            var stockpileLoc = StorageFactory.GetStockpilePosition(Job.Owner);
            cratesWithItems.Add(stockpileLoc.Position);
            bool stockpileHasItems = true;

            foreach (var item in ItemsToGet)
            {
                if (!Job.Owner.Stockpile.Contains(item))
                    stockpileHasItems = false;

                if (StorageFactory.CrateTracker.ItemCrateLocations.TryGetValue(Job.Owner, out var itemCrates) && itemCrates.TryGetValue(item.Id, out var locations))
                    if (!LastCratePosition.Contains(ItemsForGoal.ClosestCrate) && locations.Contains(ItemsForGoal.ClosestCrate))
                        cratesWithItems.Add(ItemsForGoal.ClosestCrate);
                    else
                        foreach(var loc in locations)
                            if (!LastCratePosition.Contains(loc))
                                cratesWithItems.AddIfUnique(loc);
            }

            if (cratesWithItems.Count == 0 || cratesWithItems[0] == stockpileLoc.Position)
            {
                if (stockpileHasItems)
                {
                    WalkingTo = StorageType.Stockpile;

                    if (stockpileLoc.Position == Vector3Int.invalidPos || stockpileLoc.Position == default(Vector3Int))
                        return Job.Owner.Banners.FirstOrDefault().Position;
                    else
                        return stockpileLoc.Position;
                }
                else
                {
                    PandaJobFactory.SetActiveGoal(Job, new StandAtJobGoal(Job, NextGoal, OriginalPosition, ItemsToGet.FirstOrDefault()));
                    return OriginalPosition;
                }
            }
            else
            {
                WalkingTo = StorageType.Crate;

                if (cratesWithItems.Count == 1)
                    CurrentCratePosition = cratesWithItems[0];
                else
                {
                    var pos = OriginalPosition.GetClosestPosition(cratesWithItems);

                    if (pos == stockpileLoc.Position)
                    {
                        WalkingTo = StorageType.Stockpile;
                        return stockpileLoc.Position;
                    }
                    else
                        CurrentCratePosition = pos;
                }

                return CurrentCratePosition;
            }
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

        public void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            StoredItem[] remaining = new StoredItem[0];
            state.JobIsDone = true;
            state.SetCooldown(_waitTime);

            if (remaining != null && remaining.Length != 0)
                state.SetIndicator(new Shared.IndicatorState(_waitTime, remaining.FirstOrDefault().Id.Name, true, false));

            if (WalkingTo == StorageType.Crate)
            {
                if (StorageFactory.CrateTracker.Positions.TryGetValue(CurrentCratePosition, out var ci))
                    remaining = ci.Inventory.TryTake(ItemsToGet).Values.ToArray();
            }
            else
            {
                remaining = StorageFactory.TryTakeItemsReturnRemaining(Job.Owner, ItemsToGet);
                LastCratePosition.Clear();
                WalkingTo = StorageType.Crate;
            }

            if (remaining.Length != 0)
            {
                ItemsToGet = remaining;
                LastCratePosition.Add(CurrentCratePosition);
            }
            else
            {
                PandaJobFactory.SetActiveGoal(Job, NextGoal, ref state);
            }
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return OriginalPosition;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
