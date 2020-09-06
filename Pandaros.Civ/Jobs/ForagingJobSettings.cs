using Jobs;
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
    public abstract class ForagingJobSettings : IBlockJobSettings, IPandaJobSettings
    {
        public static List<BlockJobInstance> ForagingJobs { get; set; } = new List<BlockJobInstance>();
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

        public virtual int ForagingTimeMinSec { get; set; }
        public virtual int ForagingTimeMaxSec { get; set; }
        public virtual float LuckMod { get; set; }
        public virtual ILootTable LootTable { get; set; }
        public virtual Dictionary<IJob, Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Vector3Int>();

        public virtual ItemTypes.ItemType[] BlockTypes { get; set; }

        public virtual NPCType NPCType { get; set; }

        public virtual InventoryItem RecruitmentItem { get; set; }

        public virtual bool ToSleep => TimeCycle.ShouldSleep;

        public virtual float NPCShopGameHourMinimum => TimeCycle.Settings.SleepTimeEnd;

        public virtual float NPCShopGameHourMaximum => TimeCycle.Settings.SleepTimeStart;

        public virtual Dictionary<IJob, INpcGoal> CurrentGoal { get; set; } = new Dictionary<IJob, INpcGoal>();
        public virtual event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public virtual Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!CurrentGoal.ContainsKey(instance))
                CurrentGoal.Add(instance, new ForagingGoal(instance, this, instance.Position, LootTable, ForagingTimeMinSec, ForagingTimeMaxSec, LuckMod));

            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition.Add(instance, instance.Position);

            if (!ForagingJobs.Contains(instance))
                ForagingJobs.Add(instance);

            return CurrentGoal[instance].GetPosition();
        }

        public virtual void OnGoalChanged(BlockJobInstance instance, NPCBase.NPCGoal goalOld, NPCBase.NPCGoal goalNew)
        {
            if (!instance.IsValid)
            {
                ForagingJobs.Remove(instance);
                CurrentGoal.Remove(instance);
                OriginalPosition.Remove(instance);
            }
        }

        public virtual void OnNPCAtJob(BlockJobInstance instance, ref NPCBase.NPCState state)
        {
            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition.Add(instance, instance.Position);

            CurrentGoal[instance].PerformGoal(ref state);
        }

        public virtual void OnNPCAtStockpile(BlockJobInstance instance, ref NPCBase.NPCState state)
        {
            
        }

        public virtual void SetGoal(IJob job, INpcGoal npcGoal, ref NPCBase.NPCState state)
        {
            var oldGoal = CurrentGoal[job];

            if (oldGoal != null)
                oldGoal.LeavingGoal();

            state.JobIsDone = true;
            CurrentGoal[job] = npcGoal;
            npcGoal.SetAsGoal();
            GoalChanged?.Invoke(this, (oldGoal, npcGoal));
        }
    }
}
