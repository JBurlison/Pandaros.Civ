﻿using Pandaros.Civ.Storage;
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
    public class PutItemsInCrateGoal : INpcGoal
    {

        public PutItemsInCrateGoal(IJob job, IPandaJobSettings jobSettings, INpcGoal nextGoal, StoredItem[] itemsToStore, INpcGoal goalStoring)
        {
            Job = job;
            NextGoal = nextGoal;
            JobSettings = jobSettings;
            ItemsToStore = itemsToStore;
            GoalStoring = goalStoring;
        }

        public PutItemsInCrateGoal(IJob job, IPandaJobSettings jobSettings, INpcGoal nextGoal, List<InventoryItem> itemsToStore, INpcGoal goalStoring)
        {
            Job = job;
            NextGoal = nextGoal;
            JobSettings = jobSettings;
            ItemsToStore = itemsToStore.Select(i => new StoredItem(i)).ToArray();
            GoalStoring = goalStoring;
        }

        public Vector3Int ClosestCrate { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public StoredItem[] ItemsToStore { get; set; }
        public INpcGoal GoalStoring { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public PandaGoalJob Porter { get; set; }
        public string Name { get; set; } = nameof(PutItemsInCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(PutItemsInCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        float _waitTime = Pipliz.Random.NextFloat(8, 16);

        public virtual Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                if (!LastCratePosition.Contains(GoalStoring.ClosestCrate) && StorageFactory.CrateLocations[Job.Owner].ContainsKey(GoalStoring.ClosestCrate))
                    CurrentCratePosition = GoalStoring.ClosestCrate;
                else
                {
                    var locations = JobSettings.OriginalPosition[Job].SortClosestPositions(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());

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
                        CurrentCratePosition = StorageFactory.GetStockpilePosition(Job.Owner).Position;
                    }
                }
            }

            if (CurrentCratePosition == Vector3Int.invalidPos)
            {
                WalkingTo = StorageType.Stockpile;
                CurrentCratePosition = StorageFactory.GetStockpilePosition(Job.Owner).Position;
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
            state.SetCooldown(4);

            if (ItemsToStore != null && ItemsToStore.Length != 0)
                state.SetIndicator(new Shared.IndicatorState(_waitTime, ItemsToStore.FirstOrDefault().Id.Name));

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
                JobSettings.SetGoal(Job, NextGoal, ref state);
        }
    }
}
