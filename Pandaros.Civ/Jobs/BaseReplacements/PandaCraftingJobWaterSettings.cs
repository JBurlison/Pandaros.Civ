using Jobs;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaCraftingJobWaterSettings : CraftingJobWaterSettings
    {
        public PandaCraftingJobWaterSettings(CraftingJobWaterSettings settings) :
            base(settings.BlockTypes.FirstOrDefault().Name, settings.NPCTypeString, settings.Cooldown, settings.MaxGatheredBeforeCrate, settings.OnCraftedAudio)
        {

        }

        public PandaCraftingJobWaterSettings(string blockType, string npcType, float cooldown, int maxCraftsPerHaul, string onCraftedAudio) : 
            base(blockType, npcType, cooldown, maxCraftsPerHaul, onCraftedAudio)
        {
        }

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
            {
                goal = goal = new WaterGatherGoal(instance as CraftingJobWaterInstance, this);
                PandaJobFactory.SetActiveGoal(instance, goal);
            }

            return goal.GetPosition();
        }

        public override void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
        {
            PandaJobFactory.ActiveGoals[blockInstance.Owner][blockInstance].PerformGoal(ref state);
        }
    }
}
