using NPC;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ
{
    public static class NPCExtentions
    {
        public static IPandaJob GetPandaJob(this NPCBase nPCBase)
        {
            if (nPCBase.CustomData.TryGetAs<int>("PandaJobId", out int jobId) &&
                PandaJobFactory.TakenJobs.TryGetValue(jobId, out var job))
                return job;

            return null;
        }
    }
}
