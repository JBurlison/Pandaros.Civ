using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.API;

namespace Pandaros.Civ.Jobs.Goals
{
    public class PutItemsInCrateGoal : INpcGoal
    {

        public PutItemsInCrateGoal(IPandaJob job, INpcGoal nextGoal, StoredItem[] itemsToStore)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToStore = itemsToStore;
        }

        public PutItemsInCrateGoal(IPandaJob job, INpcGoal nextGoal, List<InventoryItem> itemsToStore)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToStore = itemsToStore.Select(i => new StoredItem(i)).ToArray();
        }

        public StoredItem[] ItemsToStore { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IPandaJob Job { get; set; }
        public string Name { get; set; } = nameof(PutItemsInCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(PutItemsInCrateGoal));
        public Vector3Int CurrentCratePosition { get; set; }
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        float _waitTime = Pipliz.Random.NextFloat(8, 16);

        public virtual Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                var locations = Job.NPC.Position.SortClosestPositions(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());

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

            return CurrentCratePosition;
        }

        public virtual void LeavingGoal()
        {
            
        }

        public virtual void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            StoredItem[] remaining = new StoredItem[0];

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
                Job.SetGoal(NextGoal);
        }
    }
}
