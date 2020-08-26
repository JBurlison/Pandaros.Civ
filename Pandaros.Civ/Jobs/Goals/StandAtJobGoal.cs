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
        public StandAtJobGoal(IPandaJob job, Vector3Int pos)
        {
            Job = job;
            Position = pos;
        }

        public IPandaJob Job { get; set; }
        public string Name { get; set; } = nameof(StandAtJobGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(StandAtJobGoal));
        public Vector3Int Position { get; set; }

        public Vector3Int GetPosition()
        {
            return Position;
        }

        public void LeavingGoal()
        {
            
        }

        public void PerformGoal(ref NPC.NPCBase.NPCState state)
        {
            state.SetCooldown(5);
        }
    }
}
