using Jobs;
using ModLoaderInterfaces;
using NPC;
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.StoneAge.Items;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pandaros.Civ.Jobs.PandaGoalJob;

namespace Pandaros.Civ.Jobs
{
    public abstract class PorterJobSettings : IBlockJobSettings
    {
        public PorterJobSettings(string blockType, string npcTypeKey, PorterJobType storageType)
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
            StorageType = storageType;
        }

        public PorterJobType StorageType { get; set; }

        public virtual ItemTypes.ItemType[] BlockTypes { get; set; }

        public virtual NPCType NPCType { get; set; }

        public virtual InventoryItem RecruitmentItem { get; set; }

        public virtual bool ToSleep => TimeCycle.ShouldSleep;

        public virtual float NPCShopGameHourMinimum => TimeCycle.Settings.SleepTimeEnd;

        public virtual float NPCShopGameHourMaximum => TimeCycle.Settings.SleepTimeStart;

        public virtual Vector3Int GetJobLocation(BlockJobInstance instance)
        {
            if (!PandaJobFactory.TryGetActiveGoal(instance, out var goal))
                if (StorageType == PorterJobType.ToCrate)
                {
                    var stc = new StockpikeToCrateGoal(instance);
                    PandaJobFactory.SetActiveGoal(instance, stc);
                    return stc.GetPosition();
                }
                else
                {
                    var cts = new CrateToStockpikeGoal(instance);
                    PandaJobFactory.SetActiveGoal(instance, cts);
                    return cts.GetPosition();
                }

            return goal.GetPosition();
        }

        public void OnGoalChanged(BlockJobInstance instance, INPCGoal goalOld, INPCGoal goalNew)
        {
           
        }

        public virtual void OnNPCAtJob(BlockJobInstance instance, ref NPCBase.NPCState state)
        {
            PandaJobFactory.ActiveGoals[instance.Owner][instance].PerformGoal(ref state);
        }

        public virtual void OnNPCAtStockpile(BlockJobInstance instance, ref NPCBase.NPCState state)
        {
            
        }

    }
}
