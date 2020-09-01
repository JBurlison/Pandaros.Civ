using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaCraftingJobRotatedSettings : CraftingJobRotatedSettings, IPandaJobSettings
    {
        public PandaCraftingJobRotatedSettings(CraftingJobRotatedSettings settings) : base(settings.BlockTypes[0].Name, settings.NPCTypeKey)
        {
            BlockTypes = settings.BlockTypes;
            CraftingCooldown = settings.CraftingCooldown;
            MaxCraftsPerHaul = settings.MaxCraftsPerHaul;
            NPCType = settings.NPCType;
            NPCTypeKey = settings.NPCTypeKey;
            OnCraftedAudio = settings.OnCraftedAudio;
            RecruitmentItem = settings.RecruitmentItem;

        }

        public PandaCraftingJobRotatedSettings(string blockType, string npcTypeKey, float craftingCooldown = 5, int maxCraftsPerHaul = 5, string onCraftedAudio = null) :
            base(blockType, npcTypeKey, craftingCooldown, maxCraftsPerHaul, onCraftedAudio)
        {

        }

        public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();
        public Dictionary<IJob, Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Vector3Int>();

        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public override Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!CurrentGoal.TryGetValue(instance, out var goal))
            {
                goal = new CraftingRotatedGoal(instance, this, this);
                CurrentGoal.Add(instance, goal);
            }

            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition.Add(instance, instance.Position);

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

        public override void OnGoalChanged(BlockJobInstance instanceBlock, NPCBase.NPCGoal oldGoal, NPCBase.NPCGoal newGoal)
        {
            if (!instanceBlock.IsValid)
            {
                CurrentGoal[instanceBlock].LeavingJob();
                CurrentGoal.Remove(instanceBlock);
            }

            base.OnGoalChanged(instanceBlock, oldGoal, newGoal);
        }
    }
}
