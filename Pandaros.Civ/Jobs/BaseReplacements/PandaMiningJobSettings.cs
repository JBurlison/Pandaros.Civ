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
    public class PandaMiningJobSettings : MinerJobSettings
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

        public PandaMiningJobSettings()
        {
        }

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
		{
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
            {
                goal = new MiningGoal(instance, this);
                PandaJobFactory.SetActiveGoal(instance, goal);
            }

            return goal.GetPosition();
        }

		public override void OnNPCAtJob(BlockJobInstance blockJobInstance, ref NPCBase.NPCState state)
		{
            PandaJobFactory.ActiveGoals[blockJobInstance.Owner][blockJobInstance].PerformGoal(ref state);
        }
    }
}
