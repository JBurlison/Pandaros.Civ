using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class StandAtJobGoal : INpcGoal
    {
        public StandAtJobGoal(IPandaJob job, INpcGoal nextGoal, Vector3Int pos)
        {
            Job = job;
            Position = pos;
            NextGoal = nextGoal;
        }

        public INpcGoal NextGoal { get; set; }
        public IPandaJob Job { get; set; }
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

        public void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            if (!HasWaited)
            {
                state.SetCooldown(16);
                HasWaited = true;
            }
            else
                Job.SetGoal(NextGoal);
        }
    }
}
