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
        ///     Old goal, new goal
        /// </summary>
        event Action<IPandaJob, INpcGoal, INpcGoal> GoalChanged;
        INpcGoal CurrentGoal { get; }
        string LocalizationKey { get; set; }
        void SetGoal(INpcGoal npcGoal);
        string JobBlock { get; set; }
        IPandaJob GetNewJob(Colony c, Vector3Int pos, string nPCType, InventoryItem recruitmentItem, string jobBlock, bool sleepNight = true);
    }
}
