using BlockEntities;
using BlockTypes;
using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public class PandaJob : IPandaJob
    {
        public PandaJob(Colony c, Vector3Int pos, string npcTypekey, InventoryItem recruitmentItem, string jobBlock, INpcGoal startingGoal, bool sleepNight = true)
        {
            Owner = c;
            NPCType = NPCType.GetByKeyNameOrDefault(npcTypekey);
            SetGoal(startingGoal);
            SleepAtNight = sleepNight;
            RecruitmentItem = recruitmentItem;
            JobBlock = jobBlock;
            Position = pos;
        }

        public INpcGoal CurrentGoal { get; set; }
        public Colony Owner { get; set; }
        public string LocalizationKey { get; set; }
        public NPCBase NPC { get; set; }
        public NPCType NPCType { get; set; }
        public bool SleepAtNight { get; set; }
        public Vector3Int Position { get; set; }
        public float NPCShopGameHourMinimum => TimeCycle.Settings.SleepTimeEnd;

        public float NPCShopGameHourMaximum => TimeCycle.Settings.SleepTimeStart;

        public bool NeedsNPC => NPC == null;

        public InventoryItem RecruitmentItem { get; set; }

        public bool IsValid { get; set; } = true;
        public string JobBlock { get; set; }

        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public virtual void SetGoal(INpcGoal npcGoal)
        {
            var oldGoal = CurrentGoal;

            if (oldGoal != null)
                oldGoal.LeavingGoal();

            CurrentGoal = npcGoal;
            GoalChanged?.Invoke(this, (oldGoal, npcGoal));
        }


        public virtual void SetNPC(NPCBase npc)
        {
            var oldNpc = NPC;
            NPC = npc;

            if (npc != null)
            {
                npc.TakeJob(this);
            }
        }

        public virtual Vector3Int GetJobLocation()
        {
            return CurrentGoal.GetPosition();
        }

        public virtual void OnNPCAtJob(ref NPCBase.NPCState state)
        {
            CurrentGoal.PerformGoal(ref state);
        }

        public virtual void JobRemoved()
        {
            SetGoal(null);
        }

        public virtual NPCBase.NPCGoal CalculateGoal(ref NPCBase.NPCState state)
        {
            var nPCGoal = NPCBase.NPCGoal.Job;

            if (SleepAtNight && !TimeCycle.IsDay)
            {
                nPCGoal = NPCBase.NPCGoal.Bed;
            }
            else if (TimeCycle.IsDay && !SleepAtNight)
            {
                nPCGoal = NPCBase.NPCGoal.Bed;
            }

            return nPCGoal;
        }

        public virtual void OnNPCAtStockpile(ref NPCBase.NPCState state)
        {

        }

        public virtual void OnNPCCouldNotPathToGoal()
        {
            
        }

        public virtual EKeepChunkLoadedResult OnKeepChunkLoaded(Vector3Int blockPosition)
        {
            return EKeepChunkLoadedResult.YesLong;
        }
    }
}
