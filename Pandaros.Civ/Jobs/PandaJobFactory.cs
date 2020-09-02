using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jobs;
using Jobs.Implementations;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.Civ.Extender;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pipliz;
using Pipliz.JSON;
using static AreaJobTracker.AreaJobPatches;
using static Jobs.BlockFarmAreaJobDefinition;
using static Pandaros.Civ.Jobs.BaseReplacements.PandaBlockFarmAreaJobDefinition;

namespace Pandaros.Civ.Jobs
{
    [ModLoader.ModManager]
    public class PandaJobFactory : IOnRegisteringEntityManagers, IAfterItemTypesDefined
    {
        [ModLoader.ModCallbackDependsOn(GameInitializer.NAMESPACE + ".SettlerManager.OnNPCJobChanged")]
        public void OnRegisteringEntityManagers(List<object> managers)
        {
            foreach (var manager in managers)
            {
                if (manager is BlockJobManager<MinerJobInstance> mji)
                    mji.Settings = new PandaMiningJobSettings(mji.Settings as MinerJobSettings);
                else if (manager is BlockJobManager<GuardJobInstance> gji)
                    gji.Settings = new PandaGuardJobSettings(gji.Settings as GuardJobSettings);
                else if (manager is BlockJobManager<CraftingJobWaterInstance> cjwi)
                    cjwi.Settings = new PandaCraftingJobWaterSettings(cjwi.Settings as CraftingJobWaterSettings);
                else if (manager is BlockJobManager<CraftingJobInstance> cji)
                {
                    if (cji.Settings is CraftingJobRotatedLitSettings cjrls)
                        cji.Settings = new PandaCraftingJobRotatedLitSettings(cjrls);
                    else if (cji.Settings is CraftingJobRotatedSettings cjrs)
                        cji.Settings = new PandaCraftingJobRotatedSettings(cjrs);
                    else
                        cji.Settings = new PandaCrafingSettings(cji.Settings as CraftingJobSettings);
                }

            }
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".Jobs.PandaJobFactory.AfterItemTypesDefined")]
        [ModLoader.ModCallbackDependsOn("areajobs.insertattributed")]
        [ModLoader.ModCallbackDependsOn("createareajobdefinitions")]
        [ModLoader.ModCallbackProvidesFor("pipliz.server.endloadcolonies")]
        public void AfterItemTypesDefined()
        {
            foreach(var k in AreaJobTracker.RegisteredAreaJobDefinitions.Keys.ToList())
            {
                var val = AreaJobTracker.RegisteredAreaJobDefinitions[k];

                if (val is SimpleFarmJob sfj)
                {
                    AreaJobTracker.RegisteredAreaJobDefinitions[k] = new PandaSimpleFarmJob(sfj.Identifier, sfj.UsedNPCType, sfj.Stages, true, sfj.MaxGathersPerRun);
                }
                else if (val is BlockFarmAreaJobDefinition bfaj)
                {
                    AreaJobTracker.RegisteredAreaJobDefinitions[k] = new PandaBlockFarmAreaJobDefinition(bfaj.Identifier, bfaj.NPCTypeString, bfaj.Cooldown, bfaj.MaxGathersPerRun, bfaj.RequiredBlockItem, bfaj.PlacedBlockType);
                }
            }
        }
    }
}
