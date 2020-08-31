using Jobs;
using NPC;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaFarmingJob : FarmAreaJob, IPandaJobSettings
    {
		
		public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();
		public Dictionary<IJob, Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Vector3Int>();

		public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

		public void SetGoal(IJob job, INpcGoal npcGoal, ref NPCBase.NPCState state)
		{
			var oldGoal = CurrentGoal[job];

			if (oldGoal != null)
				oldGoal.LeavingGoal();

			state.JobIsDone = true;
			CurrentGoal[job] = npcGoal;
			npcGoal.SetAsGoal();
			GoalChanged?.Invoke(this, (oldGoal, npcGoal));
		}

        public override Vector3Int GetJobLocation()
        {
            return base.GetJobLocation();
        }


        public PandaFarmingJob(AbstractFarmAreaJobDefinition definition, Colony owner, Vector3Int min, Vector3Int max, int npcID = 0) : base(definition, owner, min, max, npcID)
        {
        }

		public override void OnNPCAtJob(ref NPCBase.NPCState state)
		{
			
		}
	}
}
