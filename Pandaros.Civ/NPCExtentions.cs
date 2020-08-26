using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static NPC.NPCBase;

namespace Pandaros.Civ
{
    public static class NPCExtentions
    {
        public static IPandaJob GetPandaJob(this NPCBase nPCBase)
        {
            if (nPCBase.CustomData.TryGetAs<int>("PandaJobId", out int jobId) &&
                PandaJobFactory.TakenJobs.TryGetValue(nPCBase.Colony, out var jobDic) &&
                jobDic.TryGetValue(jobId, out var job))
                return job;

            return null;
        }
    }
}
