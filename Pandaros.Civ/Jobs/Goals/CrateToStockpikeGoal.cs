using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class CrateToStockpikeGoal : INpcGoal
    {
        public static List<Vector3Int> InProgress { get; set; } = new List<Vector3Int>();

 
        public CrateToStockpikeGoal(IJob job, IPandaJobSettings jobSettings)
        {
            Job = job;
            JobSettings = jobSettings;
            PorterJob = job as PorterJob;
        }

        public PorterJob PorterJob { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(CrateToStockpikeGoal);
        public string LocalizationKey { get; set; }
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        public StoredItem[] ToStockpike { get; set; }

        public Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                var locations = Job.NPC.Position.SortClosestPositions(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());

                foreach (var location in locations)
                    if (!LastCratePosition.Contains(location) &&
                        !InProgress.Contains(location) &&
                        StorageFactory.CrateLocations[Job.Owner][location].IsAlmostFull &&
                        StorageFactory.CrateLocations[Job.Owner][location].StorageTypeLookup[StorageType.Stockpile].Count > 1)
                    {
                        CurrentCratePosition = location;
                        InProgress.Add(location);
                        break;
                    }

                // No new goal. go back to job pos.
                if (CurrentCratePosition == Vector3Int.invalidPos)
                {
                    CurrentCratePosition = PorterJob.OriginalPosition;
                }

                return CurrentCratePosition;
            }
            else
            {
                return StorageFactory.GetStockpilePosition(Job.Owner).Position;
            }
        }

        public void LeavingGoal()
        {
           
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            if (WalkingTo == StorageType.Crate)
            {
                if (CurrentCratePosition == PorterJob.OriginalPosition)
                {
                    state.SetCooldown(10);
                    JobSettings.SetGoal(Job, new StockpikeToCrateGoal(Job, JobSettings));
                }
                else
                {
                    state.SetCooldown(5);
                    state.SetIndicator(new Shared.IndicatorState(5, ColonyBuiltIn.ItemTypes.CRATE.Id));
                    var crate = StorageFactory.CrateLocations[Job.Owner][CurrentCratePosition];
                    ToStockpike = crate.StorageTypeLookup[StorageType.Stockpile].ToArray();
                    crate.TryTake(ToStockpike);
                    WalkingTo = StorageType.Stockpile;
                }
            }
            else
            {
                state.SetCooldown(5);
                state.SetIndicator(new Shared.IndicatorState(5, ColonyBuiltIn.ItemTypes.CRATE.Id));
                StorageFactory.StoreItems(Job.Owner, ToStockpike);
                ToStockpike = null;
                WalkingTo = StorageType.Crate;
                InProgress.Remove(CurrentCratePosition);
                CurrentCratePosition = Vector3Int.invalidPos;
            }
        }
    }
}
