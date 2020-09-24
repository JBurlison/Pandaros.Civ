using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Storage;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class FarmingGoal : IPandaNpcGoal
    {
        public FarmingGoal(PandaFarmingJob job, AbstractFarmAreaJobDefinition definitioan)
        {
            FarmingJob = job;
            Job = job;
            Definition = definitioan;
        }

        public AbstractFarmAreaJobDefinition Definition { get; set; }
        public PandaFarmingJob FarmingJob { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(FarmingGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(FarmingGoal));
        public Vector3Int ClosestCrate { get; set; }

        private int firstIndexToCheck = 0;

        private static List<ItemTypes.ItemTypeDrops> GatherResults = new List<ItemTypes.ItemTypeDrops>();

        public Vector3Int GetPosition()
        {
            if (StorageFactory.CrateLocations.TryGetValue(Job.Owner, out var crateLocs) &&
                (ClosestCrate == default(Vector3Int) || !crateLocs.ContainsKey(ClosestCrate)))
                ClosestCrate = StorageFactory.GetClosestCrateLocation(FarmingJob.KeyLocation, Job.Owner);

            if (!FarmingJob.PositionSub.IsValid)
            {
                FarmingJob.CalculateSubPosition();
            }
            return FarmingJob.PositionSub;
        }

        public void LeavingGoal()
        {

        }

        public void LeavingJob()
        {

        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            ThreadManager.AssertIsMainThread();
            ushort[] stages = Definition.Stages;
            state.JobIsDone = true;
            if (stages == null || stages.Length < 2 || !FarmingJob.PositionSub.IsValid)
            {
                state.SetCooldown(0.8, 1.2);
                FarmingJob.PositionSub = Vector3Int.invalidPos;
                return;
            }
            if (!World.TryGetTypeAt(FarmingJob.PositionSub, out ushort type))
            {
                state.SetCooldown(Pipliz.Random.NextFloat(3f, 6f));
                FarmingJob.PositionSub = Vector3Int.invalidPos;
                return;
            }
            ushort typeSeeds = stages[0];
            ushort typeFinal = stages[stages.Length - 1];

            if (type == 0)
            {
                if (World.TryGetTypeAt(FarmingJob.PositionSub.Add(0, -1, 0), out ItemTypes.ItemType typeBelow) && typeBelow.IsFertile)
                {
                    if (!Definition.JobRequiresSeeds || state.Inventory.TryGetOneItem(typeSeeds) || Job.NPC.Colony.Stockpile.TryRemove(typeSeeds))
                    {
                        ServerManager.TryChangeBlock(FarmingJob.PositionSub, type, typeSeeds, Job.Owner, ESetBlockFlags.DefaultAudio);
                        state.SetCooldown(0.8, 1.2);
                    }
                    else
                    {
                        state.SetIndicator(new IndicatorState(2f, typeSeeds, striked: true, green: false));
                        if (state.Inventory.UsedCapacity > 0f)
                        {
                            PutItemsInCrate(ref state);
                        }
                    }
                }
                else
                {
                    state.SetCooldown(3.0, 6.0);
                }
            }
            else if (type == typeFinal)
            {
                if (ServerManager.TryChangeBlock(FarmingJob.PositionSub, type, 0, Job.Owner, ESetBlockFlags.DefaultAudio) == EServerChangeBlockResult.Success)
                {
                    GatherResults.Clear();
                    List<ItemTypes.ItemTypeDrops> results = ItemTypes.GetType(typeFinal).OnRemoveItems;
                    for (int i = 0; i < results.Count; i++)
                    {
                        GatherResults.Add(results[i]);
                    }
                    FarmingJob.GatheredItemsCount++;
                    if (FarmingJob.GatheredItemsCount >= Definition.MaxGathersPerRun)
                    {
                        PutItemsInCrate(ref state);
                        FarmingJob.GatheredItemsCount = 0;
                    }
                    if (firstIndexToCheck > 0)
                    {
                        firstIndexToCheck--;
                    }

                    ModLoader.Callbacks.OnNPCGathered.Invoke(Job, FarmingJob.PositionSub, GatherResults);
                    Job.NPC.Inventory.Add(GatherResults);
                }
                state.SetCooldown(0.8, 1.2);
            }
            else
            {
                if (state.Inventory.UsedCapacity > 0f)
                {
                    PutItemsInCrate(ref state);
                }
                state.SetCooldown(4.0, 6.0);
            }
            FarmingJob.PositionSub = Vector3Int.invalidPos;
        }

        public virtual void PutItemsInCrate(ref NPCBase.NPCState state)
        {
            PandaJobFactory.SetActiveGoal(Job, new PutItemsInCrateGoal(Job, FarmingJob.KeyLocation, this, state.Inventory.Inventory.ToList(), this), ref state);
            state.Inventory.Inventory.Clear();
            state.SetCooldown(0.2, 0.4);
        }

        public void SetAsGoal()
        {
            
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return FarmingJob.KeyLocation;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }
    }
}
