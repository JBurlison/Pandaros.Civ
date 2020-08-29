using Jobs;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public class PorterJob : BlockJobInstance
    {
        public Vector3Int OriginalPosition { get; private set; }

        public PorterJob(IBlockJobSettings settings, Vector3Int position, ItemTypes.ItemType type, ByteReader reader) : base(settings, position, type, reader)
        {
            OriginalPosition = position;
        }

        public PorterJob(IBlockJobSettings settings, Vector3Int position, ItemTypes.ItemType type, Colony colony) : base(settings, position, type, colony)
        {
            OriginalPosition = position;
        }

        public override Vector3Int GetJobLocation()
        {
            return Settings.GetJobLocation(this);
        }

        public override void OnNPCAtJob(ref NPCBase.NPCState state)
        {
            Settings.OnNPCAtJob(this, ref state);
        }

        public override void OnNPCCouldNotPathToGoal()
        {
            NPC.SetPosition(OriginalPosition);
            NPC.SendPositionUpdate();
        }

        public override NPCBase.NPCGoal CalculateGoal(ref NPCBase.NPCState state)
        {
            return CalculateGoal(ref state, true);
        }

        public NPCBase.NPCGoal CalculateGoal(ref NPCBase.NPCState state, bool sleepAtNight)
        {
            var nPCGoal = NPCBase.NPCGoal.Job;

            if (sleepAtNight && !TimeCycle.IsDay)
            {
                nPCGoal = NPCBase.NPCGoal.Bed;
            }
            else if (TimeCycle.IsDay && !sleepAtNight)
            {
                nPCGoal = NPCBase.NPCGoal.Bed;
            }

            if (nPCGoal != LastNPCGoal)
            {
                Settings.OnGoalChanged(this, LastNPCGoal, nPCGoal);
                LastNPCGoal = nPCGoal;
            }

            return nPCGoal;
        }

        public void SetGoal(IJob job, INpcGoal npcGoal)
        {
           
        }
    }
}
