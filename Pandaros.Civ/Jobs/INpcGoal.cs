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

        IPandaJob Job { get; set; }
        string Name { get; set; }
        string LocalizationKey { get; set; }
        Vector3Int GetPosition();

        void PerformGoal(ref NPC.NPCBase.NPCState state);

        void SetJob(IPandaJob job);

        void LeavingGoal();
    }
}
