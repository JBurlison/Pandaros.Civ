using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class CrateToStockpikeGoal : IPandaNpcGoal
    {
        public static List<Vector3Int> InProgress { get; set; } = new List<Vector3Int>();

 
        public CrateToStockpikeGoal(IJob job)
        {
            Job = job;
            PorterJob = job as PandaGoalJob;
        }

        public Vector3Int ClosestCrate { get; set; }
        public PandaGoalJob PorterJob { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(CrateToStockpikeGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(CrateToStockpikeGoal));
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();
        public List<Vector3Int> ClosestLocations { get; set; } = new List<Vector3Int>();
        public StorageType WalkingTo { get; set; } = StorageType.Crate;
        public StoredItem[] ToStockpike { get; set; }

        public Vector3Int GetCrateSearchPosition()
        {
            return PorterJob.OriginalPosition;
        }
        public Vector3Int GetPosition()
        {
            if (WalkingTo == StorageType.Crate)
            {
                // check for full crates. ensure they get serviced first
                foreach (var location in ClosestLocations)
                    if (!LastCratePosition.Contains(location) &&
                        !InProgress.Contains(location) &&
                        StorageFactory.CrateTracker.Positions.TryGetValue(location, out var crate) &&
                        crate.Inventory.IsAlmostFull &&
                        crate.Inventory.StorageTypeLookup[StorageType.Stockpile].Count > 0)
                    {
                        CurrentCratePosition = location;
                        InProgress.Add(location);
                        break;
                    }

                // No new goal. just take anything
                if (CurrentCratePosition == Vector3Int.invalidPos)
                {
                    foreach (var location in ClosestLocations)
                        if (!LastCratePosition.Contains(location) &&
                            !InProgress.Contains(location) &&
                            StorageFactory.CrateTracker.Positions.TryGetValue(location, out var crate) &&
                            crate.Inventory.StorageTypeLookup[StorageType.Stockpile].Count > 0)
                        {
                            CurrentCratePosition = location;
                            InProgress.Add(location);
                            break;
                        }
                }

                // everything is empty stand at job.
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
            state.SetCooldown(1);

            if (WalkingTo == StorageType.Crate)
            {
                if (StorageFactory.CrateTracker.Positions.TryGetValue(CurrentCratePosition, out var crate))
                {
                    ToStockpike = crate.Inventory.GetAllItems(StorageType.Stockpile).Values.ToArray();
                    ShowIndicator(ref state);
                    crate.Inventory.TryTake(ToStockpike);
                    WalkingTo = StorageType.Stockpile;
                }
                else
                {
                    LastCratePosition.Clear();
                }

            }
            else
            {
                ShowIndicator(ref state);
                StorageFactory.StoreItems(Job.Owner, ToStockpike);
                ToStockpike = null;
                WalkingTo = StorageType.Crate;
                InProgress.Remove(CurrentCratePosition);
                CurrentCratePosition = Vector3Int.invalidPos;
                GetPosition();
            }
        }

        private void  ShowIndicator(ref NPCBase.NPCState state)
        {
            if (ToStockpike != null && ToStockpike.Length > 0)
                state.SetIndicator(new Shared.IndicatorState(2, ToStockpike[0].Id.Id));
            else
                state.SetIndicator(new Shared.IndicatorState(2, ColonyBuiltIn.ItemTypes.CRATE.Id));
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
