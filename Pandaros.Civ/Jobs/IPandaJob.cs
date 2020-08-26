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
using Pandaros.API.Extender;
using BlockEntities;

namespace Pandaros.Civ.Jobs
{
    public interface IPandaJob : IJob, IBlockEntityKeepLoaded
    {
        /// <summary>
        ///     Old npc, new npc
        /// </summary>
        event Action<IPandaJob, NPCBase, NPCBase> NPCSet;
        /// <summary>
        ///     Old goal, new goal
        /// </summary>
        event Action<IPandaJob, INpcGoal, INpcGoal> GoalChanged;
        int JobId { get; set; }
        INpcGoal CurrentGoal { get; }
        string LocalizationKey { get; set; }
        string JobBlock { get; set; }
        void SetGoal(INpcGoal npcGoal);

    }
}
