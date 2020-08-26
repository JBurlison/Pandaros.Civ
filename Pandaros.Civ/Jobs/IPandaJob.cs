using Jobs;
using NPC;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;

namespace Pandaros.Civ.Jobs
{
    public interface IPandaJob : IOnUpdate
    {
        /// <summary>
        ///     Old npc, new npc
        /// </summary>
        event EventHandler<(NPCBase, NPCBase)> NPCSet;
        /// <summary>
        ///     Old goal, new goal
        /// </summary>
        event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;
        int JobId { get; set; }
        INpcGoal CurrentGoal { get; }
        Colony Owner { get; set; }
        string LocalizationKey { get; set; }
        NPCBase NPC { get; set; }
        NPCType NPCType { get; set; }

        void SetGoal(INpcGoal npcGoal);
        void SetNPC(NPCBase npc);

    }
}
