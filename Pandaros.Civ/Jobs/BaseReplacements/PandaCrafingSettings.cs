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
    public class PandaCrafingSettings : CraftingJobSettings, IPandaJobSettings
    {
        public PandaCrafingSettings(CraftingJobSettings settings)
        {
            BlockTypes = settings.BlockTypes;
            CraftingCooldown = settings.CraftingCooldown;
            MaxCraftsPerHaul = settings.MaxCraftsPerHaul;
            NPCType = settings.NPCType;
            NPCTypeKey = settings.NPCTypeKey;
            OnCraftedAudio = settings.OnCraftedAudio;
            RecruitmentItem = settings.RecruitmentItem;
        }

        public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();
        public Dictionary<IJob, Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Vector3Int>();

        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public override Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!CurrentGoal.ContainsKey(instance))
                CurrentGoal.Add(instance, new CraftingGoal(instance, this, this));

            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition.Add(instance, instance.Position);

            return CurrentGoal[instance].GetPosition();
        }

        public override void OnNPCAtJob(BlockJobInstance blockJobInstance, ref NPCBase.NPCState state)
        {
            if (!OriginalPosition.ContainsKey(blockJobInstance))
                OriginalPosition.Add(blockJobInstance, blockJobInstance.Position);

            CurrentGoal[blockJobInstance].PerformGoal(ref state);
        }

        public void SetGoal(IJob job, INpcGoal npcGoal)
        {
            var oldGoal = CurrentGoal[job];

            if (oldGoal != null)
                oldGoal.LeavingGoal();

            CurrentGoal[job] = npcGoal;
            npcGoal.SetAsGoal();
            GoalChanged?.Invoke(this, (oldGoal, npcGoal));
        }

        public override void OnGoalChanged(BlockJobInstance instanceBlock, NPCBase.NPCGoal oldGoal, NPCBase.NPCGoal newGoal)
        {
            if (!instanceBlock.IsValid)
                CurrentGoal[instanceBlock].LeavingJob();

            base.OnGoalChanged(instanceBlock, oldGoal, newGoal);
        }
    }
}
