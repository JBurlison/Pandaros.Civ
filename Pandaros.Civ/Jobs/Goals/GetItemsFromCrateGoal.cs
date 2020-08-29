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
        public static List<GetItemsFromCrateGoal> CurrentItemsNeeded { get; set; } = new List<GetItemsFromCrateGoal>();

        public GetItemsFromCrateGoal(IJob job, IPandaJobSettings jobSettings, INpcGoal nextGoal, StoredItem[] itemsToGet)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet;
            JobSettings = jobSettings;
            CurrentItemsNeeded.Add(this);
        }

        public GetItemsFromCrateGoal(IJob job, IPandaJobSettings jobSettings, INpcGoal nextGoal, List<InventoryItem> itemsToGet)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet.Select(i => new StoredItem(i)).ToArray();
            JobSettings = jobSettings;
            CurrentItemsNeeded.Add(this);
        }

        public void SetJob(IJob job)
        {
            Job = job;
        }

        public StoredItem[] ItemsToGet { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public string Name { get; set; } = nameof(GetItemsFromCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(GetItemsFromCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; }
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        float _waitTime = Pipliz.Random.NextFloat(8, 16);

        public Vector3Int GetPosition()
        {
            List<Vector3Int> cratesWithItems = new List<Vector3Int>();

            foreach (var item in ItemsToGet)
            {
                if (StorageFactory.ItemCrateLocations[Job.Owner].TryGetValue(item.Id, out var locations))
                    foreach(var loc in locations)
                        if (!LastCratePosition.Contains(loc))
                            cratesWithItems.AddIfUnique(loc);
            }


            if (cratesWithItems.Count == 0)
            {
                WalkingTo = StorageType.Stockpile;
                return StorageFactory.GetStockpilePosition(Job.Owner).Position;
            }
            else
            {
                CurrentCratePosition = Job.NPC.Position.GetClosestPosition(cratesWithItems);
                return CurrentCratePosition;
            }
        }

        public void LeavingGoal()
        {
            CurrentItemsNeeded.Remove(this);
        }

        public virtual void SetAsGoal()
        {
            CurrentItemsNeeded.Add(this);
        }

        public void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            StoredItem[] remaining = new StoredItem[0];

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
                state.SetCooldown(_waitTime);
                state.SetIndicator(new Shared.IndicatorState(_waitTime, remaining.FirstOrDefault().Id.Name, true, false));
                ItemsToGet = remaining;
                LastCratePosition.Add(CurrentCratePosition);
            }
            else
            {
                state.SetCooldown(0.2, 0.4);
                JobSettings.SetGoal(Job, NextGoal);
            }
        }
    }
}
