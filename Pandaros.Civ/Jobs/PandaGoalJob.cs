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
    public class PandaGoalJob : BlockJobInstance
    {
        public enum PorterJobType
        {
            FromCrate,
            ToCrate
        }

        public Vector3Int OriginalPosition { get; private set; }

        public PandaGoalJob(IBlockJobSettings settings, Vector3Int position, ItemTypes.ItemType type, ByteReader reader) : base(settings, position, type, reader)
        {
            OriginalPosition = position;
        }

        public PandaGoalJob(IBlockJobSettings settings, Vector3Int position, ItemTypes.ItemType type, Colony colony) : base(settings, position, type, colony)
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

        public void SetGoal(IJob job, IPandaNpcGoal npcGoal)
        {
           
        }
    }
}
