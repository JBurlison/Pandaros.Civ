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
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.Storage;
using Pipliz;
using Pipliz.JSON;
using static AreaJobTracker.AreaJobPatches;
using static Jobs.BlockFarmAreaJobDefinition;
using static Pandaros.Civ.Jobs.BaseReplacements.PandaBlockFarmAreaJobDefinition;

namespace Pandaros.Civ.Jobs
{
    [ModLoader.ModManager]
    public class PandaJobFactory : IOnRegisteringEntityManagers, IAfterItemTypesDefined, IOnNPCJobChanged, IOnTimedUpdate, ICratePlacementUpdate, IOnQuit
    {
        public static Dictionary<Colony, Dictionary<IJob, IPandaNpcGoal>> ActiveGoals { get; set; } = new Dictionary<Colony, Dictionary<IJob, IPandaNpcGoal>>();
        public static Dictionary<string, List<IPandaNpcGoal>> ActiveGoalsByType { get; set; } = new Dictionary<string, List<IPandaNpcGoal>>();
        public static Dictionary<string, PandaGuardJobSettings> GuardJobsSettings { get; set; } = new Dictionary<string, PandaGuardJobSettings>();
        public static Dictionary<string, PandaMiningJobSettings> MineJobsSettings { get; set; } = new Dictionary<string, PandaMiningJobSettings>();
        public int NextUpdateTimeMinMs => 10;

        public int NextUpdateTimeMaxMs => 15;

        public ServerTimeStamp NextUpdateTime { get; set; }

        public void CratePlacementUpdate(Colony colony, PlacementEventType eventType, Vector3Int position)
        {
            if (eventType == PlacementEventType.Removed)
            {
                foreach (var goalType in ActiveGoalsByType.Values)
                    foreach (var goal in goalType)
                        if (goal.ClosestCrate == position)
                            goal.ClosestCrate = StorageFactory.GetClosestCrateLocation(goal.GetCrateSearchPosition(), colony);
            }
            else
            {
                foreach (var goalType in ActiveGoalsByType.Values)
                    foreach (var goal in goalType)
                        goal.ClosestCrate = StorageFactory.GetClosestCrateLocation(goal.GetCrateSearchPosition(), colony);
            }
        }

        public void OnTimedUpdate()
        {
            List<Colony> found = new List<Colony>();
            List<Colony> notFound = new List<Colony>();
            foreach (var c in ServerManager.ColonyTracker.ColoniesByID.Values)
                if (c != null && ActiveGoals.ContainsKey(c) && !found.Contains(c))
                    found.Add(c);

            foreach (var c in ActiveGoals.Keys)
                if (!found.Contains(c))
                    notFound.Add(c);

            foreach (var n in notFound)
            {
                if (ActiveGoals.TryGetValue(n, out var jobGoalDict))
                    foreach (var goal in jobGoalDict.Values)
                        if (ActiveGoalsByType.TryGetValue(goal.Name, out var goalList))
                            goalList.Remove(goal);

                ActiveGoals.Remove(n);
            }
        }

        public static bool TryGetActiveGoal(IJob job, out IPandaNpcGoal goal)
        {
            if (ActiveGoals.TryGetValue(job.Owner, out var goalList) && goalList.TryGetValue(job, out goal))
            {
                return true;
            }

            goal = default(IPandaNpcGoal);
            return false;
        }

        public static bool HasGoal(IJob job)
        {
            return ActiveGoals.TryGetValue(job.Owner, out var jobGoals) && jobGoals.ContainsKey(job);
        }

        public static void SetActiveGoal(IJob job, IPandaNpcGoal npcGoal, ref NPCBase.NPCState state)
        {
            state.JobIsDone = true;
            SetActiveGoal(job, npcGoal);
        }

        public static void SetActiveGoal(IJob job, IPandaNpcGoal npcGoal)
        {
            if (!ActiveGoalsByType.TryGetValue(npcGoal.Name, out var goals))
            {
                goals = new List<IPandaNpcGoal>();
                ActiveGoalsByType[npcGoal.Name] = goals;
            }

            if (!goals.Contains(npcGoal))
                goals.Add(npcGoal);

            if (ActiveGoals.TryGetValue(job.Owner, out var goalList))
            {
                if (goalList.TryGetValue(job, out var oldGoal))
                    oldGoal.LeavingGoal();

                goalList[job] = npcGoal;
            }
            else
            {
                goalList = new Dictionary<IJob, IPandaNpcGoal>()
                {
                    { job,  npcGoal }
                };
                ActiveGoals[job.Owner] = goalList;
            }

            npcGoal.SetAsGoal();
        }

        public static void SetGoalAsInactive(IJob job)
        {
            if (ActiveGoals.TryGetValue(job.Owner, out var goalList))
            {
                if (goalList.TryGetValue(job, out var goal) && ActiveGoalsByType.TryGetValue(goal.Name, out var goals))
                    goals.Remove(goal);

                goalList.Remove(job);
            }
        }

        public void OnNPCJobChanged((NPCBase npc, IJob oldJob, IJob newJob) tuple)
        {
            if (tuple.oldJob != null && ActiveGoals.TryGetValue(tuple.npc.Colony, out var jobGoals) && jobGoals.TryGetValue(tuple.oldJob, out var goal))
            {
                goal.LeavingGoal();
            }
        }

        [ModLoader.ModCallbackDependsOn(GameInitializer.NAMESPACE + ".SettlerManager.OnNPCJobChanged")]
        public void OnRegisteringEntityManagers(List<object> managers)
        {
            foreach (var manager in managers)
            {
                if (manager is BlockJobManager<MinerJobInstance> mji)
                {
                    if (!(mji.Settings is PandaMiningJobSettings pms))
                    {
                        var mineJobSettings = new PandaMiningJobSettings(mji.Settings as MinerJobSettings);
                        mji.Settings = mineJobSettings;
                        MineJobsSettings[mineJobSettings.NPCTypeKey] = mineJobSettings;
                    }
                    else
                        MineJobsSettings[pms.NPCTypeKey] = pms;
                }
                else if (manager is BlockJobManager<GuardJobInstance> gji)
                {
                    if (!(gji.Settings is PandaGuardJobSettings pgs))
                    {
                        var guardSettings = new PandaGuardJobSettings(gji.Settings as GuardJobSettings);
                        gji.Settings = guardSettings;
                        GuardJobsSettings[guardSettings.NPCTypeKey] = guardSettings;
                    }
                    else
                        GuardJobsSettings[pgs.NPCTypeKey] = pgs;
                }
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

        public void OnQuit()
        {
            if (PandaJobFactory.ActiveGoalsByType.TryGetValue(nameof(ForagingGoal), out var goals))
            {
                foreach (var goal in goals)
                {
                    goal.LeavingJob();
                }
            }
        }
    }
}
