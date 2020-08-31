using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jobs;
using Jobs.Implementations;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.Civ.Extender;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnRegisteringEntityManagers
    {

        public void OnRegisteringEntityManagers(List<object> managers)
        {
            foreach (var manager in managers)
            {
                if (manager is BlockJobManager<MinerJobInstance> mji)
                    mji.Settings = new PandaMiningJobSettings(mji.Settings as MinerJobSettings);
                else if (manager is BlockJobManager<CraftingJobInstance> cji)
                    cji.Settings = new PandaCrafingSettings(cji.Settings as CraftingJobSettings);

            }
        }
    }
}
