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
    public class GetItemsFromCrateGoal : INpcGoal
    {

        public GetItemsFromCrateGoal(IJob job, Vector3Int originalPos, INpcGoal nextGoal, StoredItem[] itemsToGet, INpcGoal itemsForGoal)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet;
            ItemsForGoal = itemsForGoal;
            OriginalPosition = originalPos;
        }

        public GetItemsFromCrateGoal(IJob job, Vector3Int originalPos, INpcGoal nextGoal, List<InventoryItem> itemsToGet, INpcGoal itemsForGoal)
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
        public INpcGoal ItemsForGoal { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(GetItemsFromCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(GetItemsFromCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; }
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        float _waitTime = Pipliz.Random.NextFloat(8, 16);

        public Vector3Int GetPosition()
        {
            List<Vector3Int> cratesWithItems = new List<Vector3Int>();
            var stockpileLoc = StorageFactory.GetStockpilePosition(Job.Owner);
            cratesWithItems.Add(stockpileLoc.Position);

            foreach (var item in ItemsToGet)
            {
                if (StorageFactory.ItemCrateLocations[Job.Owner].TryGetValue(item.Id, out var locations))
                    if (!LastCratePosition.Contains(ItemsForGoal.ClosestCrate) && locations.Contains(ItemsForGoal.ClosestCrate))
                        cratesWithItems.Add(ItemsForGoal.ClosestCrate);
                    else
                        foreach(var loc in locations)
                            if (!LastCratePosition.Contains(loc))
                                cratesWithItems.AddIfUnique(loc);
            }

            if (cratesWithItems.Count == 0 || cratesWithItems[0] == stockpileLoc.Position)
            {
                WalkingTo = StorageType.Stockpile;
                return stockpileLoc.Position;
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
                if (StorageFactory.CrateLocations[Job.Owner].TryGetValue(CurrentCratePosition, out CrateInventory ci))
                    remaining = ci.TryTake(ItemsToGet).Values.ToArray();
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
