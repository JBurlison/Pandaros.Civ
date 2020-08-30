using Jobs;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public abstract class PorterJobSettings : IBlockJobSettings, IPandaJobSettings
    {
        public PorterJobSettings(string blockType, string npcTypeKey)
        {
            if (blockType != null)
            {
                BlockTypes = new ItemTypes.ItemType[1]
                {
                ItemTypes.GetType(blockType)
                };
            }

            NPCType = NPCType.GetByKeyNameOrDefault(npcTypeKey);
            RecruitmentItem = new InventoryItem(LeafBag.NAME);
        }
        public Dictionary<IJob, Vector3Int> OriginalPosition { get; set; } = new Dictionary<IJob, Vector3Int>();

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
                CurrentGoal.Add(instance, new StockpikeToCrateGoal(instance, this));

            if (!OriginalPosition.ContainsKey(instance))
                OriginalPosition.Add(instance, instance.Position);

            return CurrentGoal[instance].GetPosition();
        }

        public virtual void OnGoalChanged(BlockJobInstance instance, NPCBase.NPCGoal goalOld, NPCBase.NPCGoal goalNew)
        {
            
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
