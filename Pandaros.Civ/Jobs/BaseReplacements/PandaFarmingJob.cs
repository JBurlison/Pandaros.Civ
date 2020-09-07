using Jobs;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{

	public class PandaSimpleFarmJob : AbstractFarmAreaJobDefinition
	{
		protected bool ShouldTurnArableIntoDirt { get; set; }

		public PandaSimpleFarmJob(
		  string identifier,
		  NPCType npcType,
		  ushort[] stages,
		  bool turnArableIntoDirt,
		  int maxGathersPerRun)
		{
			Identifier = identifier;
			UsedNPCType = npcType;
			Stages = stages;
			ShouldTurnArableIntoDirt = turnArableIntoDirt;
			MaxGathersPerRun = maxGathersPerRun;
		}

		public override IAreaJob CreateAreaJob(
		  Colony owner,
		  Vector3Int min,
		  Vector3Int max,
		  bool isLoaded,
		  int npcID = 0)
		{
			if (!isLoaded && this.ShouldTurnArableIntoDirt)
				TurnArableIntoDirt(min, max, owner);
			return new PandaFarmingJob((AbstractFarmAreaJobDefinition)this, owner, min, max, npcID);
		}
	}

	public class PandaFarmingJob : FarmAreaJob
    {
		public override Vector3Int GetJobLocation()
		{
			if (!PandaJobFactory.TryGetActiveGoal(this, out var goal))
			{
				goal = new FarmingGoal(this, Definition as AbstractFarmAreaJobDefinition);
				PandaJobFactory.SetActiveGoal(this, goal);
			}

			return goal.GetPosition();
		}


		public PandaFarmingJob(AbstractFarmAreaJobDefinition definition, Colony owner, Vector3Int min, Vector3Int max, int npcID = 0) : base(definition, owner, min, max, npcID)
        {
        }

		public override void OnNPCAtJob(ref NPCBase.NPCState state)
		{
			PandaJobFactory.ActiveGoals[this.Owner][this].PerformGoal(ref state);
		}
	}
}
