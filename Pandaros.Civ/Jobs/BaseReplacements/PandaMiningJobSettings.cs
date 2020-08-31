using BlockTypes;
using Jobs;
using Jobs.Implementations;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaMiningJobSettings : MinerJobSettings, IPandaJobSettings
    {
        public PandaMiningJobSettings(MinerJobSettings minerJobSettings)
        {
            BlockTypes = minerJobSettings.BlockTypes;
            MaxCraftsPerRun = minerJobSettings.MaxCraftsPerRun;
            NPCType = minerJobSettings.NPCType;
            NPCTypeKey = minerJobSettings.NPCTypeKey;
            OnCraftedAudio = minerJobSettings.OnCraftedAudio;
            RecruitmentItem = minerJobSettings.RecruitmentItem;
        }

        public Dictionary<IJob, Pipliz.Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Pipliz.Vector3Int>();
		public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();

		public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
		{
			if (!OriginalPosition.ContainsKey(instance))
				OriginalPosition[instance] = instance.Position;

            if (!CurrentGoal.TryGetValue(instance, out var goal))
            {
                goal = new MiningGoal(instance, this);
                CurrentGoal[instance] = goal;
            }

			return goal.GetPosition();
		}

		public override void OnNPCAtJob(BlockJobInstance blockJobInstance, ref NPCBase.NPCState state)
		{
			CurrentGoal[blockJobInstance].PerformGoal(ref state);
		}

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

        public override void OnGoalChanged(BlockJobInstance instance, NPCBase.NPCGoal oldGoal, NPCBase.NPCGoal newGoal)
        {
            if (!instance.IsValid)
            {
                CurrentGoal[instance].LeavingJob();
                CurrentGoal.Remove(instance);
            }

            base.OnGoalChanged(instance, oldGoal, newGoal);
        }
    }
}
