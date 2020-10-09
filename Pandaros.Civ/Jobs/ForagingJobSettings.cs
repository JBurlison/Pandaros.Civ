using Jobs;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pandaros.Civ.Jobs.PandaGoalJob;

namespace Pandaros.Civ.Jobs
{
    public abstract class ForagingJobSettings : IBlockJobSettings
    {
        public ForagingJobSettings(string blockType, string npcTypeKey, ILootTable lootTable, int foragingTimeMinSec, int foragingTimeMaxSec, float lootLuckModifier = 0f)
        {
            if (blockType != null)
            {
                BlockTypes = new ItemTypes.ItemType[1]
                {
                    ItemTypes.GetType(blockType)
                };
            }

            NPCType = NPCType.GetByKeyNameOrDefault(npcTypeKey);
            RecruitmentItem = new InventoryItem(LeafBasket.NAME);
            LootTable = lootTable;
            ForagingTimeMaxSec = foragingTimeMaxSec;
            ForagingTimeMinSec = foragingTimeMinSec;
            LuckMod = lootLuckModifier;
        }

        public string BlockType { get; set; }
        public virtual int ForagingTimeMinSec { get; set; }
        public virtual int ForagingTimeMaxSec { get; set; }
        public virtual float LuckMod { get; set; }
        public virtual ILootTable LootTable { get; set; }
        public virtual ItemTypes.ItemType[] BlockTypes { get; set; }

        public virtual NPCType NPCType { get; set; }

        public virtual InventoryItem RecruitmentItem { get; set; }

        public virtual bool ToSleep => TimeCycle.ShouldSleep;

        public virtual float NPCShopGameHourMinimum => TimeCycle.Settings.SleepTimeEnd;

        public virtual float NPCShopGameHourMaximum => TimeCycle.Settings.SleepTimeStart;

        public virtual Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
            {
                goal = new ForagingGoal(instance, instance.Position, LootTable, ForagingTimeMinSec, ForagingTimeMaxSec, LuckMod);
                PandaJobFactory.SetActiveGoal(instance, goal);
            }

            return goal.GetPosition();
        }

        public void OnGoalChanged(BlockJobInstance instance, INPCGoal goalOld, INPCGoal goalNew)
        {
            
        }

        public virtual void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
        {
            PandaJobFactory.ActiveGoals[blockInstance.Owner][blockInstance].PerformGoal(ref state);
        }

        public virtual void OnNPCAtStockpile(BlockJobInstance instance, ref NPCBase.NPCState state)
        {
            
        }

    }
}
