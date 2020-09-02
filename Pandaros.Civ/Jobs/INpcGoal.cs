using Jobs;
using ModLoaderInterfaces;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface INpcGoal
    {
        IJob Job { get; set; }
        IPandaJobSettings JobSettings { get; set; }
        string Name { get; set; }
        string LocalizationKey { get; set; }
        Vector3Int ClosestCrate { get; set; }
        Vector3Int GetPosition();

        void PerformGoal(ref NPC.NPCBase.NPCState state);

        void LeavingGoal();
        void SetAsGoal();

        void LeavingJob();
    }
}
