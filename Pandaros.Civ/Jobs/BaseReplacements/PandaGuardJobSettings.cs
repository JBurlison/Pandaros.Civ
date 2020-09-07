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
    public class PandaGuardJobSettings : GuardJobSettings
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

        public PandaGuardJobSettings()
        {
        }

        public PandaGuardJobSettings(GuardJobSettingData data) : base(data)
        {
        }

        public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
            {
                goal = new GuardGoal(instance as GuardJobInstance, this);
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
