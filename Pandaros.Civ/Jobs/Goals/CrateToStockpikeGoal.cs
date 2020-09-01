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
        public List<Vector3Int> ClosestLocations { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        public StoredItem[] ToStockpike { get; set; }

        public Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                foreach (var location in ClosestLocations)
                    if (!LastCratePosition.Contains(location) &&
                        !InProgress.Contains(location) &&
                        StorageFactory.CrateLocations[Job.Owner].TryGetValue(location, out var inv) &&
                        inv.IsAlmostFull &&
                        inv.StorageTypeLookup[StorageType.Stockpile].Count > 0)
                    {
                        CurrentCratePosition = location;
                        InProgress.Add(location);
                        break;
                    }

                // No new goal. go back to job pos.
                if (CurrentCratePosition == Vector3Int.invalidPos)
                {
                    return PorterJob.OriginalPosition;
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

        public virtual void SetAsGoal()
        {
            
        }

        public virtual void LeavingJob()
        {

        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            state.JobIsDone = true;

            if (WalkingTo == StorageType.Crate)
            {
                state.SetCooldown(5);

                if (StorageFactory.CrateLocations.TryGetValue(Job.Owner, out var locs))
                {
                    if (locs.TryGetValue(CurrentCratePosition, out var crate))
                    {
                        state.SetIndicator(new Shared.IndicatorState(5, ColonyBuiltIn.ItemTypes.CRATE.Id));
                        ToStockpike = crate.StorageTypeLookup[StorageType.Stockpile].ToArray();
                        crate.TryTake(ToStockpike);
                        WalkingTo = StorageType.Stockpile;
                    }
                    else
                    {
                        state.SetCooldown(10);
                        JobSettings.SetGoal(Job, new StockpikeToCrateGoal(Job, JobSettings), ref state);
                    }
                }
                else
                    CivLogger.Log(ChatColor.red, "Crate locations does not contain colony id {0}", Job.Owner.ColonyID);
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
                GetPosition();
            }
        }
    }
}
