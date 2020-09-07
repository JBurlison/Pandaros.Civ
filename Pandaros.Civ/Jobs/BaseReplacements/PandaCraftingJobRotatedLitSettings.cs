﻿using Jobs;
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
    public class PandaCraftingJobRotatedLitSettings : CraftingJobRotatedLitSettings
    {
        public PandaCraftingJobRotatedLitSettings(CraftingJobRotatedLitSettings settings) : base(settings.BlockTypes[0].Name, settings.NPCTypeKey)
        {
            BlockTypes = settings.BlockTypes;
            CraftingCooldown = settings.CraftingCooldown;
            MaxCraftsPerHaul = settings.MaxCraftsPerHaul;
            NPCType = settings.NPCType;
            NPCTypeKey = settings.NPCTypeKey;
            OnCraftedAudio = settings.OnCraftedAudio;
            RecruitmentItem = settings.RecruitmentItem;

        }

        public PandaCraftingJobRotatedLitSettings(string blockType, string npcTypeKey, float craftingCooldown = 5, int maxCraftsPerHaul = 5, string onCraftedAudio = null) :
            base(blockType, npcTypeKey, craftingCooldown, maxCraftsPerHaul, onCraftedAudio)
        {

        }


        public override Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
            {
                goal = new CraftingLitGoal(instance, this);
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
