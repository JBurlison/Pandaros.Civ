using BlockTypes;
using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Storage;
using Pipliz;
using Recipes;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Pandaros.Civ.Jobs.BaseReplacements.PandaBlockFarmAreaJobDefinition;

namespace Pandaros.Civ.Jobs.Goals
{

    public class BlockFarmGoal : IPandaNpcGoal
    {
        public BlockFarmGoal(PandaBlockFarmAreaJob job, PandaBlockFarmAreaJobDefinition definitioan)
        {
            FarmingJob = job;
            Job = job;
            Definition = definitioan;
        }

        List<RecipeResult> CraftingResults = new List<RecipeResult>();
        public PandaBlockFarmAreaJobDefinition Definition { get; set; }
        public PandaBlockFarmAreaJob FarmingJob { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(BlockFarmGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(BlockFarmGoal));
        public Vector3Int ClosestCrate { get; set; }
        public Recipe.RecipeMatch RecipeMatch { get; set; }

        public Vector3Int GetPosition()
        {
            if (ClosestCrate == default(Vector3Int) || !StorageFactory.CrateTracker.Positions.TryGetValue(ClosestCrate, out var crate))
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
            state.JobIsDone = true;

            if (!FarmingJob.PositionSub.IsValid)
            {
                state.SetCooldown(8.0, 12.0);
            }
            else
            {
                BlockFarmAreaJobDefinition definition = (BlockFarmAreaJobDefinition)this.Definition;
                Vector3Int vector3Int = FarmingJob.BlockLocation.IsValid ? FarmingJob.BlockLocation : FarmingJob.PositionSub;
                FarmingJob.PositionSub = Vector3Int.invalidPos;
                ItemTypes.ItemType val1;
                if (!World.TryGetTypeAt(vector3Int, out val1))
                {
                    state.SetCooldown(8.0, 12.0);
                }
                else
                {
                    if (val1 == definition.PlacedBlockType)
                    {
                        RecipeMatch = Recipe.MatchRecipe(FarmingJob.Owner.RecipeData.GetAvailableRecipes(definition.NPCTypeString), FarmingJob.Owner, FarmingJob.Owner.RecipeData.GetRecipeGroup(FarmingJob.CraftingGroupID));
                        switch (RecipeMatch.MatchType)
                        {
                            case Recipe.RecipeMatchType.FoundCraftable:
                                Recipe foundRecipe = RecipeMatch.FoundRecipe;
                                if (FarmingJob.NPC.Inventory.TryRemove(foundRecipe.Requirements))
                                {
                                    CraftingResults.Clear();

                                    for (int index = 0; index < foundRecipe.Results.Count; ++index)
                                        CraftingResults.Add(foundRecipe.Results[index]);

                                    ModLoader.Callbacks.OnNPCCraftedRecipe.Invoke(Job, foundRecipe, CraftingResults);
                                    RecipeResult weightedRandom = RecipeResult.GetWeightedRandom(CraftingResults);
                                    float timeToShow = definition.Cooldown * Pipliz.Random.NextFloat(0.9f, 1.1f);

                                    if (weightedRandom.Amount > 0)
                                        state.SetIndicator(new IndicatorState(timeToShow, weightedRandom.Type));
                                    else
                                        state.SetCooldown(timeToShow);

                                    FarmingJob.NPC.Inventory.Add(CraftingResults);
                                    ++FarmingJob.GatherCount;

                                    if (FarmingJob.GatherCount < definition.MaxGathersPerRun)
                                        return;

                                    FarmingJob.GatherCount = 0;
                                    PutItemsInCrate(ref state);
                                    return;
                                }
                                else
                                {
                                    PandaJobFactory.SetActiveGoal(Job, new GetItemsFromCrateGoal(Job, FarmingJob.KeyLocation, this, foundRecipe.Requirements, this), ref state);
                                    FarmingJob.NPC.Inventory.Add(foundRecipe.Requirements, RecipeMatch.FoundRecipeCount);
                                }
                                state.SetCooldown(0.4, 0.6);
                                return;
                            case Recipe.RecipeMatchType.FoundMissingRequirements:
                            case Recipe.RecipeMatchType.AllDone:
                                if (state.Inventory.IsEmpty)
                                {
                                    state.JobIsDone = true;
                                    float cooldown = definition.Cooldown;
                                    if (RecipeMatch.MatchType == Recipe.RecipeMatchType.AllDone)
                                        state.SetIndicator(new IndicatorState(cooldown, BuiltinBlocks.Indices.erroridle));
                                    else
                                        state.SetIndicator(new IndicatorState(cooldown, RecipeMatch.FoundRecipe.FindMissingType(FarmingJob.Owner.Stockpile), true, false));
                                    FarmingJob.Owner.Stats.RecordNPCIdleSeconds(FarmingJob.NPCType, cooldown);
                                    return;
                                }
                                PutItemsInCrate(ref state);
                                return;
                        }
                    }
                    if (val1 == BuiltinBlocks.Types.air)
                    {
                        ItemTypes.ItemType val2;
                        if (World.TryGetTypeAt(vector3Int.Add(0, -1, 0), out val2))
                        {
                            if (!definition.PlacedBlockType.RequiresFertileBelow || val2.IsFertile)
                            {
                                if (definition.RequiredBlockItem.Amount != 0 && Job.NPC.Colony.Stockpile.TryRemove(definition.RequiredBlockItem))
                                {
                                    state.SetCooldown(1.5, 2.5);
                                    PandaJobFactory.SetActiveGoal(Job, new GetItemsFromCrateGoal(Job, FarmingJob.KeyLocation, this, new[] { new StoredItem(definition.RequiredBlockItem) }, this), ref state);
                                    Job.NPC.Inventory.Add(definition.RequiredBlockItem);
                                }

                                if (definition.RequiredBlockItem.Amount == 0)
                                {
                                    ServerManager.TryChangeBlock(vector3Int, BuiltinBlocks.Types.air, definition.PlacedBlockType, Job.Owner, ESetBlockFlags.DefaultAudio);
                                        state.SetCooldown(1.5, 2.5);
                                        return;
                                }
                                state.SetIndicator(new IndicatorState(Pipliz.Random.NextFloat(8f, 14f), definition.RequiredBlockItem.Type, true, false));
                                return;
                            }
                        }
                        else
                        {
                            state.SetCooldown(8.0, 12.0);
                            return;
                        }
                    }
                    state.SetCooldown((double)Pipliz.Random.NextFloat(3f, 6f));
                }
            }
        }

        public void SetAsGoal()
        {
           
        }

        public virtual void PutItemsInCrate(ref NPCBase.NPCState state)
        {
            PandaJobFactory.SetActiveGoal(Job, new PutItemsInCrateGoal(Job, FarmingJob.KeyLocation, this, state.Inventory.Inventory.ToList(), this), ref state);
            state.Inventory.Inventory.Clear();
            state.SetCooldown(0.2, 0.4);
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return FarmingJob.KeyLocation;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            var items = new Dictionary<ushort, StoredItem>();

            if (RecipeMatch.FoundRecipe != null)
                items.AddRange(RecipeMatch.FoundRecipe.Requirements);

            return items;
        }
    }
}
