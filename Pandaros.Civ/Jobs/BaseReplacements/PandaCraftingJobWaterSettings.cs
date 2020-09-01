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
    public class PandaCraftingJobWaterSettings : CraftingJobWaterSettings, IPandaJobSettings
    {
        public PandaCraftingJobWaterSettings(CraftingJobWaterSettings settings) :
            base(settings.BlockTypes.FirstOrDefault().Name, settings.NPCTypeString, settings.Cooldown, settings.MaxGatheredBeforeCrate, settings.OnCraftedAudio)
        {

        }

        public PandaCraftingJobWaterSettings(string blockType, string npcType, float cooldown, int maxCraftsPerHaul, string onCraftedAudio) : 
            base(blockType, npcType, cooldown, maxCraftsPerHaul, onCraftedAudio)
        {
        }

        public Dictionary<IJob, Pipliz.Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Pipliz.Vector3Int>();
        public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();

        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!CurrentGoal.TryGetValue(instance, out var goal))
            {
                goal = new WaterGatherGoal(instance as CraftingJobWaterInstance, this);
                CurrentGoal[instance] = goal;
            }

            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition[instance] = instance.Position;

            return goal.GetPosition();
        }

        public override void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
        {
            if (!CurrentGoal.TryGetValue(blockInstance, out var goal))
            {
                goal = new WaterGatherGoal(blockInstance as CraftingJobWaterInstance, this);
                CurrentGoal[blockInstance] = goal;
            }

            goal.PerformGoal(ref state);
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
