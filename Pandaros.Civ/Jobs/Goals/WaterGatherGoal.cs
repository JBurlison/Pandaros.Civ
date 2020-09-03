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
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class WaterGatherRequest : ICrateRequest, ICratePlacementUpdate
    {
        public void CratePlacementUpdate(Colony colony, PlacementEventType eventType, Vector3Int position)
        {
            if (eventType == PlacementEventType.Removed)
            {
                foreach (var goal in WaterGatherGoal.CurrentlyGathering)
                    if (goal.ClosestCrate == position)
                        goal.ClosestCrate = goal.JobInstance.Position.GetClosestPosition(StorageFactory.CrateLocations[colony].Keys.ToList());
            }
            else
            {
                foreach (var goal in WaterGatherGoal.CurrentlyGathering)
                    goal.ClosestCrate = goal.JobInstance.Position.GetClosestPosition(StorageFactory.CrateLocations[colony].Keys.ToList());
            }
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded(Vector3Int crateLocation)
        {
            var items = new Dictionary<ushort, StoredItem>();

            foreach (var gatherer in WaterGatherGoal.CurrentlyGathering)
            {
                if (StorageFactory.CrateLocations.TryGetValue(gatherer.Job.Owner, out var crateLocs))
                {
                    if (!crateLocs.ContainsKey(gatherer.ClosestCrate))
                        gatherer.ClosestCrate = gatherer.JobInstance.Position.GetClosestPosition(crateLocs.Keys.ToList());

                    if (gatherer.ClosestCrate == crateLocation)
                    {
                        var maxSize = crateLocs[crateLocation].CrateType.MaxCrateStackSize;

                        if (gatherer.recipeMatch.FoundRecipe != null)
                            items.AddRange(gatherer.recipeMatch.FoundRecipe.Requirements, maxSize);
                    }
                }
            }

            return items;
        }
    }

    public class WaterGatherGoal : INpcGoal
    {
        public static List<WaterGatherGoal> CurrentlyGathering { get; set; } = new List<WaterGatherGoal>();

        public WaterGatherGoal(CraftingJobWaterInstance blockJobInstance, PandaCraftingJobWaterSettings pandaJobSettings)
        {
            JobInstance = blockJobInstance;
            Job = blockJobInstance;
            JobSettings = pandaJobSettings;
            WaterSettings = pandaJobSettings;
            CurrentlyGathering.Add(this);
        }

        public Vector3Int ClosestCrate { get; set; }
        public List<RecipeResult> CraftingResults { get; set; } = new List<RecipeResult>();
        public PandaCraftingJobWaterSettings WaterSettings { get; set; }
        public CraftingJobWaterInstance JobInstance { get; set; }
        public IJob Job { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }
        public Recipe.RecipeMatch recipeMatch { get; set; }

        public Vector3Int GetPosition()
        {
            if (StorageFactory.CrateLocations.TryGetValue(Job.Owner, out var crateLocs) &&
                (ClosestCrate == default(Vector3Int) || !crateLocs.ContainsKey(ClosestCrate)))
                ClosestCrate = JobInstance.Position.GetClosestPosition(crateLocs.Keys.ToList());

            return JobInstance.Position;
        }

        public void LeavingGoal()
        {
            
        }

        public void LeavingJob()
        {
            CurrentlyGathering.Remove(this);
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            if (!CurrentlyGathering.Contains(this))
                CurrentlyGathering.Add(this);

            CraftingJobWaterInstance instance = JobInstance;
            Colony owner = instance.Owner;
            state.JobIsDone = true;
            if (!this.CheckWater(instance))
            {
                state.SetCooldown(0.3, 0.5);
            }
            else
            {
                int index1;
                if (WaterSettings.BlockTypes.ContainsByReference<ItemTypes.ItemType>(instance.BlockType, out index1))
                {
                    UnityEngine.Vector3 vector = instance.NPC.Position.Vector;
                    switch (index1)
                    {
                        case 1:
                            ++vector.x;
                            break;
                        case 2:
                            --vector.x;
                            break;
                        case 3:
                            ++vector.z;
                            break;
                        case 4:
                            --vector.z;
                            break;
                    }
                    instance.NPC.LookAt(vector);
                }

                recipeMatch = Recipe.MatchRecipe<AvailableRecipesEnumerator>(owner.RecipeData.GetAvailableRecipes(WaterSettings.NPCTypeString), owner);
                switch (recipeMatch.MatchType)
                {
                    case Recipe.RecipeMatchType.FoundCraftable:
                        Recipe foundRecipe = recipeMatch.FoundRecipe;
                        if (state.Inventory.TryRemove(foundRecipe.Requirements))
                        {
                            CraftingResults.Clear();
                            for (int index2 = 0; index2 < foundRecipe.Results.Count; ++index2)
                                CraftingResults.Add(foundRecipe.Results[index2]);
                            if (WaterSettings.OnCraftedAudio != null)
                                AudioManager.SendAudio(instance.Position.Vector, WaterSettings.OnCraftedAudio);
                            ModLoader.Callbacks.OnNPCCraftedRecipe.Invoke(Job, foundRecipe, CraftingResults);
                            RecipeResult weightedRandom = RecipeResult.GetWeightedRandom(CraftingResults);
                            float timeToShow = WaterSettings.Cooldown * Pipliz.Random.NextFloat(0.9f, 1.1f);
                            if (weightedRandom.Amount > 0)
                                state.SetIndicator(new IndicatorState(timeToShow, weightedRandom.Type));
                            else
                                state.SetCooldown((double)timeToShow);
                            state.Inventory.Add(CraftingResults);
                            ++instance.GatheredCount;
                            if (instance.GatheredCount < WaterSettings.MaxGatheredBeforeCrate)
                                break;
                            instance.GatheredCount = 0;
                            PutItemsInCrate(ref state);
                            break;
                        }
                        else
                        {
                            GetItemsFromCrate(ref state);
                        }
                        break;
                    case Recipe.RecipeMatchType.FoundMissingRequirements:
                    case Recipe.RecipeMatchType.AllDone:
                        if (state.Inventory.IsEmpty)
                        {
                            state.JobIsDone = true;
                            if (recipeMatch.MatchType == Recipe.RecipeMatchType.AllDone)
                                state.SetIndicator(new IndicatorState(WaterSettings.Cooldown, BuiltinBlocks.Indices.erroridle));
                            else
                                state.SetIndicator(new IndicatorState(WaterSettings.Cooldown, recipeMatch.FoundRecipe.FindMissingType(owner.Stockpile), true, false));
                            Job.Owner.Stats.RecordNPCIdleSeconds(WaterSettings.NPCType, WaterSettings.Cooldown);
                            GetItemsFromCrate(ref state);
                            break;
                        }
                        GetItemsFromCrate(ref state);
                        break;
                }
            }
        }

        public virtual void PutItemsInCrate(ref NPCBase.NPCState state)
        {
            JobSettings.SetGoal(Job, new PutItemsInCrateGoal(Job, JobSettings, this, state.Inventory.Inventory.ToList(), this), ref state);
            state.Inventory.Inventory.Clear();
            state.SetCooldown(0.2, 0.4);
        }

        public virtual void GetItemsFromCrate(ref NPCBase.NPCState state)
        {
            if (recipeMatch.FoundRecipe != null)
            {
                state.SetCooldown(0.4, 0.6);
                state.Inventory.Add(recipeMatch.FoundRecipe.Requirements.ToList(), recipeMatch.FoundRecipeCount);
                JobSettings.SetGoal(Job, new GetItemsFromCrateGoal(Job, JobSettings, this, recipeMatch.FoundRecipe.Requirements, this), ref state);
            }
        }

        private bool CheckWater(CraftingJobWaterInstance instance)
        {
            if (instance.WaterPosition != Pipliz.Vector3Int.invalidPos)
            {
                ushort val;
                if (!World.TryGetTypeAt(instance.WaterPosition, out val))
                    return false;
                if ((int)val == (int)BuiltinBlocks.Indices.water)
                    return true;
                instance.WaterPosition = Pipliz.Vector3Int.invalidPos;
            }
            UnityEngine.Assertions.Assert.IsTrue(instance.WaterPosition == Pipliz.Vector3Int.invalidPos, "waterpos wasn't invalid");
            for (int a = -1; a <= 1; ++a)
            {
                for (int b = -1; b <= 1; ++b)
                {
                    for (int c = -1; c <= 1; ++c)
                    {
                        ushort val;
                        if (!World.TryGetTypeAt(instance.Position.Add(a, b, c), out val))
                            return false;
                        if ((int)val == (int)BuiltinBlocks.Indices.water)
                        {
                            instance.WaterPosition = instance.Position.Add(a, b, c);
                            return true;
                        }
                    }
                }
            }
            int num = (int)ServerManager.TryChangeBlock(instance.Position, instance.BlockType, BuiltinBlocks.Types.air, new BlockChangeRequestOrigin(instance.Owner));
            return false;
        }

        public void SetAsGoal()
        {
            if (!CurrentlyGathering.Contains(this))
                CurrentlyGathering.Add(this);
        }
    }
}
