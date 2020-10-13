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
        public HashSet<string> MinableTypes { get; set; }
        public float MiningCooldown { get; set; }

        public PandaMiningJobSettings(MinerJobSettings minerJobSettings)
        {
            BlockTypes = minerJobSettings.BlockTypes;
            MaxCraftsPerRun = minerJobSettings.MaxCraftsPerRun;
            NPCType = minerJobSettings.NPCType;
            NPCTypeKey = minerJobSettings.NPCTypeKey;
            OnCraftedAudio = minerJobSettings.OnCraftedAudio;
            RecruitmentItem = minerJobSettings.RecruitmentItem;
        }

        public PandaMiningJobSettings(string blockType, string npcType, int maxCraftsPerRun, float miningCooldown, HashSet<string> minableTypes, string onCraftedAudio = "stoneDelete")
        {
            ItemTypes.ItemType type = ItemTypes.GetType(blockType);
            if (type.RotatedXMinus != null)
            {
                BlockTypes = new ItemTypes.ItemType[5]
                {
                    type,
                    ItemTypes.GetType(type.RotatedXPlus),
                    ItemTypes.GetType(type.RotatedXMinus),
                    ItemTypes.GetType(type.RotatedZPlus),
                    ItemTypes.GetType(type.RotatedZMinus)
                };
            }
            else
            {
                BlockTypes = new ItemTypes.ItemType[5]
                {
                    type,
                    ItemTypes.GetType(blockType + "x+"),
                    ItemTypes.GetType(blockType + "x-"),
                    ItemTypes.GetType(blockType + "z+"),
                    ItemTypes.GetType(blockType + "z-")
                };
            }

            MiningCooldown = miningCooldown;
            MinableTypes = minableTypes;
            NPCTypeKey = npcType;
            NPCType = NPC.NPCType.GetByKeyNameOrDefault(npcType);
            OnCraftedAudio = onCraftedAudio;
            MaxCraftsPerRun = maxCraftsPerRun;
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
