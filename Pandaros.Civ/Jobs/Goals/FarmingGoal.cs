//using Jobs;
//using NPC;
//using Pandaros.Civ.Jobs.BaseReplacements;
//using Pipliz;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pandaros.Civ.Jobs.Goals
//{
//    public class FarmingGoal : INpcGoal
//    {
//        public FarmingGoal(PandaFarmingJob job)
//        {
//            FarmingJob = job;
//            Job = job;
//            JobSettings = job;
//        }

//        public PandaFarmingJob FarmingJob { get; set; }
//        public IJob Job { get; set; }
//        public IPandaJobSettings JobSettings { get; set; }
//        public string Name { get; set; }
//        public string LocalizationKey { get; set; }


//        private const int CHECK_INDEX_COUNT = 100;

//        private const int CHECK_FOUND_MOVABLE_SUB = 95;

//        private int firstIndexToCheck = 0;

//        private static ItemTypes.ItemType[] yTypesBuffer = new ItemTypes.ItemType[5];

//        private static List<ItemTypes.ItemTypeDrops> GatherResults = new List<ItemTypes.ItemTypeDrops>();

//        public Vector3Int GetPosition()
//        {
//            if (!FarmingJob.positionSub.IsValid)
//            {
//                FarmingJob.CalculateSubPosition();
//            }
//            return FarmingJob.positionSub;
//        }

//        public void LeavingGoal()
//        {

//        }

//        public void LeavingJob()
//        {

//        }

//        public void PerformGoal(ref NPCBase.NPCState state)
//        {
//            ThreadManager.AssertIsMainThread();
//            AbstractFarmAreaJobDefinition def = (AbstractFarmAreaJobDefinition)definition;
//            ushort[] stages = def.Stages;
//            state.JobIsDone = true;
//            if (stages == null || stages.Length < 2 || !positionSub.IsValid)
//            {
//                state.SetCooldown(0.8, 1.2);
//                positionSub = Vector3Int.invalidPos;
//                return;
//            }
//            if (!World.TryGetTypeAt(positionSub, out ushort type))
//            {
//                state.SetCooldown(Pipliz.Random.NextFloat(3f, 6f));
//                positionSub = Vector3Int.invalidPos;
//                return;
//            }
//            ushort typeSeeds = stages[0];
//            ushort typeFinal = stages[stages.Length - 1];
//            if (type == 0)
//            {
//                if (World.TryGetTypeAt(positionSub.Add(0, -1, 0), out ItemTypes.ItemType typeBelow) && typeBelow.IsFertile)
//                {
//                    if (!def.JobRequiresSeeds || state.Inventory.TryGetOneItem(typeSeeds) || NPC.Colony.Stockpile.TryRemove(typeSeeds))
//                    {
//                        ServerManager.TryChangeBlock(positionSub, type, typeSeeds, Owner, ESetBlockFlags.DefaultAudio);
//                        state.SetCooldown(0.8, 1.2);
//                    }
//                    else
//                    {
//                        state.SetIndicator(new IndicatorState(2f, typeSeeds, striked: true, green: false));
//                        shouldDumpInventory = (state.Inventory.UsedCapacity > 0f);
//                    }
//                }
//                else
//                {
//                    state.SetCooldown(3.0, 6.0);
//                }
//            }
//            else if (type == typeFinal)
//            {
//                if (ServerManager.TryChangeBlock(positionSub, type, 0, Owner, ESetBlockFlags.DefaultAudio) == EServerChangeBlockResult.Success)
//                {
//                    GatherResults.Clear();
//                    List<ItemTypes.ItemTypeDrops> results = ItemTypes.GetType(typeFinal).OnRemoveItems;
//                    for (int i = 0; i < results.Count; i++)
//                    {
//                        GatherResults.Add(results[i]);
//                    }
//                    GatheredItemsCount++;
//                    if (GatheredItemsCount >= definition.MaxGathersPerRun)
//                    {
//                        shouldDumpInventory = true;
//                        GatheredItemsCount = 0;
//                    }
//                    if (firstIndexToCheck > 0)
//                    {
//                        firstIndexToCheck--;
//                    }
//                    ModLoader.Callbacks.OnNPCGathered.Invoke(this, positionSub, GatherResults);
//                    NPC.Inventory.Add(GatherResults);
//                }
//                state.SetCooldown(0.8, 1.2);
//            }
//            else
//            {
//                shouldDumpInventory = (state.Inventory.UsedCapacity > 0f);
//                state.SetCooldown(4.0, 6.0);
//            }
//            positionSub = Vector3Int.invalidPos;
//        }

//        public void SetAsGoal()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
