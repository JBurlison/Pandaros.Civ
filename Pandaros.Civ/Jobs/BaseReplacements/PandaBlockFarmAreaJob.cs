using BlockTypes;
using Jobs;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Pipliz.JSON;
using Recipes;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaBlockFarmAreaJobDefinition : BlockFarmAreaJobDefinition 
    {
        public PandaBlockFarmAreaJobDefinition(string identifier, string npcType, float cooldown, int maxGatheredBeforeCrate, InventoryItem requiredBlockItem, ItemTypes.ItemType placedBlockType) :
            base(identifier, npcType, cooldown, maxGatheredBeforeCrate, requiredBlockItem, placedBlockType)
        {
        }

        public override IAreaJob CreateAreaJob(
          Colony owner,
          Vector3Int min,
          Vector3Int max,
          bool isLoaded,
          int npcID = 0) => (IAreaJob)new BlockFarmAreaJobDefinition.BlockFarmAreaJob(this, owner, min, max, npcID);

        public override IAreaJob CreateAreaJob(Colony owner, JSONNode node)
        {
            Vector3Int min = new Vector3Int()
            {
                x = node.GetAsOrDefault<int>("x-", int.MinValue),
                y = node.GetAsOrDefault<int>("y-", int.MinValue),
                z = node.GetAsOrDefault<int>("z-", int.MinValue)
            };
            Vector3Int max = min + new Vector3Int()
            {
                x = node.GetAsOrDefault<int>("xd", 0),
                y = node.GetAsOrDefault<int>("yd", 0),
                z = node.GetAsOrDefault<int>("zd", 0)
            };
            int asOrDefault = node.GetAsOrDefault<int>("npc", 0);
            RecipeSettingsGroup.GroupID npcGroupID = new RecipeSettingsGroup.GroupID(node.GetAsOrDefault<uint>("gid", 0U));
            return new PandaBlockFarmAreaJob(this, owner, min, max, asOrDefault, npcGroupID);
        }

        public class PandaBlockFarmAreaJob : BlockFarmAreaJob
        {
            public Vector3Int BlockLocation { get { return blockLocation; } }
            public int GatherCount { get { return gatheredCount; } set { gatheredCount = value; } }
            public PandaBlockFarmAreaJob(
              BlockFarmAreaJobDefinition def,
              Colony owner,
              Vector3Int min,
              Vector3Int max,
              int npcID = 0)
              : base(def, owner, min, max, npcID)
              => this.CraftingGroupID = owner.RecipeData.RecipeSettingsDefault.ID;

            public PandaBlockFarmAreaJob(
              BlockFarmAreaJobDefinition def,
              Colony owner,
              Vector3Int min,
              Vector3Int max,
              int npcID,
              RecipeSettingsGroup.GroupID npcGroupID)
              : base(def, owner, min, max, npcID)
              => this.CraftingGroupID = npcGroupID;

            public override Vector3Int GetJobLocation()
            {
                if (!PandaJobFactory.TryGetActiveGoal(this, out var goal))
                {
                    goal = new BlockFarmGoal(this, Definition as PandaBlockFarmAreaJobDefinition);
                    PandaJobFactory.SetActiveGoal(this, goal);
                }

                return goal.GetPosition();
            }


            public override void OnNPCAtJob(ref NPCBase.NPCState state)
            {
                PandaJobFactory.ActiveGoals[Owner][this].PerformGoal(ref state);
            }

        }
    }
}
