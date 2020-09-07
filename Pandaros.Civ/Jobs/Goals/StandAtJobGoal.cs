using Jobs;
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
    public class StandAtJobGoal : INpcGoal
    {
        public StandAtJobGoal(IJob job, INpcGoal nextGoal, Vector3Int pos, StoredItem missingItem = null)
        {
            Job = job;
            Position = pos;
            NextGoal = nextGoal;
            MissingItem = missingItem;
        }

        public StoredItem MissingItem { get; set; }
        public Vector3Int ClosestCrate { get; set; }
        public INpcGoal NextGoal { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(StandAtJobGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(StandAtJobGoal));
        public Vector3Int Position { get; set; }
        public bool HasWaited { get; set; } = false;

        public Vector3Int GetPosition()
        {
            return Position;
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
            if (!HasWaited)
            {
                if (MissingItem != null)
                    state.SetIndicator(new Shared.IndicatorState(16f, MissingItem.Id.Id, true, false));

                state.SetCooldown(16);
                HasWaited = true;
            }
            else
                PandaJobFactory.SetActiveGoal(Job, NextGoal, ref state);
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return Position;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
