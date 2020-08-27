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
    public class GetItemsFromCrateGoal : INpcGoal
    {
        public GetItemsFromCrateGoal(IPandaJob job, INpcGoal nextGoal, StoredItem[] itemsToGet)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet;
        }

        public GetItemsFromCrateGoal(IPandaJob job, INpcGoal nextGoal, List<InventoryItem> itemsToGet)
        {
            Job = job;
            NextGoal = nextGoal;
            ItemsToGet = itemsToGet.Select(i => new StoredItem(i)).ToArray();
        }

        public StoredItem[] ItemsToGet { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IPandaJob Job { get; set; }
        public string Name { get; set; } = nameof(GetItemsFromCrateGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(GetItemsFromCrateGoal));
        public Vector3Int LastCratePosition { get; set; }

        float _waitTime = Pipliz.Random.NextFloat(8, 16);

        public Vector3Int GetPosition()
        {
            List<Vector3Int> cratesWithItems = new List<Vector3Int>();

            foreach (var item in ItemsToGet)
            {
                if (StorageFactory.ItemCrateLocations[Job.Owner].TryGetValue(item.Id, out var locations))
                    foreach(var loc in locations)
                        cratesWithItems.AddIfUnique(loc);
            }

            LastCratePosition = Job.NPC.Position.GetClosestPosition(cratesWithItems);

            return LastCratePosition;
        }

        public void LeavingGoal()
        {
            
        }

        public void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            StoredItem[] remaining = new StoredItem[0];

            if (StorageFactory.CrateLocations[Job.Owner].TryGetValue(LastCratePosition, out CrateInventory ci))
                remaining = ci.TryTake(ItemsToGet).Values.ToArray();

            if (remaining.Length != 0)
            {
                state.SetCooldown(_waitTime);
                state.SetIndicator(new Shared.IndicatorState(_waitTime, remaining.FirstOrDefault().Id.Name, true, false));
                ItemsToGet = remaining;
            }
            else
            {
                Job.SetGoal(NextGoal);
            }
        }
    }
}
