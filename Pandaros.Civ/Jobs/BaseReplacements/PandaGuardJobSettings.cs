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
    public class PandaGuardJobSettings : GuardJobSettings, IPandaJobSettings
    {
        public PandaGuardJobSettings(GuardJobSettings settings)
        {
            BlockTypes = settings.BlockTypes;
            NPCType = settings.NPCType;
            NPCTypeKey = settings.NPCTypeKey;
            OnShootAudio = settings.OnShootAudio;
            RecruitmentItem = settings.RecruitmentItem;
            ShootItem = settings.ShootItem;
            SleepType = settings.SleepType;
            Range = settings.Range;
            Damage = settings.Damage;
            CooldownShot = settings.CooldownShot;
            OnShootResultItem = settings.OnShootResultItem;
            CooldownSearchingTarget = settings.CooldownSearchingTarget;
            CooldownMissingItem = settings.CooldownMissingItem;
            OnHitAudio = settings.OnHitAudio;
        }

        public Dictionary<IJob, Pipliz.Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Pipliz.Vector3Int>();
        public Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();

        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition[instance] = instance.Position;

            return CurrentGoal[instance].GetPosition();
        }

        public override void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
        {
            if (!CurrentGoal.TryGetValue(blockInstance, out var goal))
            {
                goal = new GuardGoal(blockInstance as GuardJobInstance, this);
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
