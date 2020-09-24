using Jobs;
using ModLoaderInterfaces;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface IPandaNpcGoal

    {
        IJob Job { get; set; }
        string Name { get; set; }
        string LocalizationKey { get; set; }
        Vector3Int ClosestCrate { get; set; }
        Vector3Int GetPosition();
        Vector3Int GetCrateSearchPosition();
        Dictionary<ushort, StoredItem> GetItemsNeeded();
        void PerformGoal(ref NPC.NPCBase.NPCState state);
        void LeavingGoal();
        void SetAsGoal();
        void LeavingJob();
    }
}
