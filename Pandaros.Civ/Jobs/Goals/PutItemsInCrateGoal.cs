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
    public class PutItemsInCrateGoal : IPandaNpcGoal
    {

        public PutItemsInCrateGoal(IJob job, Vector3Int originalPos, IPandaNpcGoal nextGoal, StoredItem[] itemsToStore, IPandaNpcGoal goalStoring)
        {
            Job = job;
            NextGoal = nextGoal;
            OriginalPos = originalPos;
            ItemsToStore = itemsToStore;
            GoalStoring = goalStoring;
        }

        public PutItemsInCrateGoal(IJob job, Vector3Int originalPos, IPandaNpcGoal nextGoal, List<InventoryItem> itemsToStore, IPandaNpcGoal goalStoring)
        {
            Job = job;
            NextGoal = nextGoal;
            OriginalPos = originalPos;
            ItemsToStore = itemsToStore.Select(i => new StoredItem(i)).ToArray();
            GoalStoring = goalStoring;
        }

        public Vector3Int OriginalPos { get; set; }
        public Vector3Int ClosestCrate { get; set; }
        public StoredItem[] ItemsToStore { get; set; }
        public IPandaNpcGoal GoalStoring { get; set; }
        public IPandaNpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public PandaGoalJob Porter { get; set; }
        public string Name { get; set; } = nameof(PutItemsInCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(PutItemsInCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;

        public virtual Vector3Int GetPosition()
        {
            var stockpileLoc = StorageFactory.GetStockpilePosition(Job.Owner);

            if (WalkingTo == StorageType.Crate)
            {
                if (!LastCratePosition.Contains(GoalStoring.ClosestCrate) && StorageFactory.CrateLocations[Job.Owner].ContainsKey(GoalStoring.ClosestCrate))
                    CurrentCratePosition = GoalStoring.ClosestCrate;
                else
                {
                    var locations = GetCrateSearchPosition().SortClosestPositions(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());

                    foreach (var location in locations)
                        if (!LastCratePosition.Contains(location))
                        {
                            CurrentCratePosition = location;
                            break;
                        }

                    // we have checked every crate, they are all full.
                    // put items in stockpile.
                    if (LastCratePosition.Contains(CurrentCratePosition))
                    {
                        WalkingTo = StorageType.Stockpile;

                        if (stockpileLoc.Position == Vector3Int.invalidPos || stockpileLoc.Position == default(Vector3Int))
                            CurrentCratePosition = Job.Owner.Banners.FirstOrDefault().Position;
                        else
                            CurrentCratePosition = stockpileLoc.Position;
                    }
                }
            }

            if (CurrentCratePosition == Vector3Int.invalidPos || CurrentCratePosition == default(Vector3Int))
            {
                WalkingTo = StorageType.Stockpile;

                if (stockpileLoc.Position == Vector3Int.invalidPos || stockpileLoc.Position == default(Vector3Int))
                    CurrentCratePosition = Job.Owner.Banners.FirstOrDefault().Position;
                else
                    CurrentCratePosition = stockpileLoc.Position;
            }

            return CurrentCratePosition;
        }

        public virtual void LeavingGoal()
        {
            
        }

        public virtual void SetAsGoal()
        {

        }

        public virtual void LeavingJob()
        {
           
        }

        public virtual void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            StoredItem[] remaining = new StoredItem[0];
            state.SetCooldown(1);

            if (ItemsToStore != null && ItemsToStore.Length != 0)
                state.SetIndicator(new Shared.IndicatorState(1, ItemsToStore.FirstOrDefault().Id.Name));

            if (WalkingTo == StorageType.Crate)
            {
                if (StorageFactory.CrateLocations[Job.Owner].TryGetValue(CurrentCratePosition, out CrateInventory ci))
                    remaining = ci.TryAdd(ItemsToStore).ToArray();
            }
            else
                StorageFactory.StoreItems(Job.Owner, ItemsToStore);

            if (remaining.Length > 0)
            {
                ItemsToStore = remaining;
                LastCratePosition.Add(CurrentCratePosition);
            }
            else
                PandaJobFactory.SetActiveGoal(Job, NextGoal, ref state);
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return OriginalPos;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
