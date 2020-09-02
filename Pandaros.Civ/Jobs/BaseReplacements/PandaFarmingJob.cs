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
			if (!CurrentGoal.TryGetValue(this, out var goal))
			{
				goal = new FarmingGoal(this, Definition as AbstractFarmAreaJobDefinition);
				CurrentGoal.Add(this, goal);
			}

			if (!OriginalPosition.ContainsKey(this))
				OriginalPosition.Add(this, this.KeyLocation);

			return goal.GetPosition();
		}


		public PandaFarmingJob(AbstractFarmAreaJobDefinition definition, Colony owner, Vector3Int min, Vector3Int max, int npcID = 0) : base(definition, owner, min, max, npcID)
        {
        }

		public override void OnNPCAtJob(ref NPCBase.NPCState state)
		{
			if (!CurrentGoal.TryGetValue(this, out var goal))
			{
				goal = new FarmingGoal(this, Definition as AbstractFarmAreaJobDefinition);
				CurrentGoal.Add(this, goal);
			}

			goal.PerformGoal(ref state);
		}
	}
}
