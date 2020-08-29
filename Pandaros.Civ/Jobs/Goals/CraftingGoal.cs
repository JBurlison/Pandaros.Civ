using BlockTypes;
using Jobs;
using NPC;
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
    public class CraftingGoal : INpcGoal
    {
        public static List<CraftingGoal> CurrentlyCrafing { get; set; } = new List<CraftingGoal>();

        public CraftingGoal(IJob job, IPandaJobSettings jobSettings, CraftingJobSettings settings)
        {
            CraftingJobInstance = job as CraftingJobInstance;
            RecipeGroupID = CraftingJobInstance.CraftingGroupID;
            JobSettings = jobSettings;
            CurrentlyCrafing.Add(this);
            Job = job;
            CraftingJobSettings = settings;
            if (Job.Owner.RecipeData.TryGetRecipeGroup(RecipeGroupID, out var recipeSettingsGroup))
                RecipeSettingsGroup = recipeSettingsGroup;
            else if (Job.Owner.RecipeData.TryGetRecipeGroup(RecipeSettingsGroup.GroupID.Default, out var defaultGroup))
                RecipeSettingsGroup = defaultGroup;
        }

        public CraftingJobInstance CraftingJobInstance { get; set; }
        public CraftingJobSettings CraftingJobSettings { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public List<RecipeResult> CraftingResults { get; set; } = new List<RecipeResult>();
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(CraftingGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(CraftingGoal));
        public RecipeSettingsGroup.GroupID RecipeGroupID { get; set; }
        public RecipeSettingsGroup RecipeSettingsGroup { get; set; }
        public Recipe.RecipeMatch NextRecipe { get; set; }

        public virtual Vector3Int GetPosition()
        {
            return ((BlockJobInstance)Job).Position;
        }

        public virtual void LeavingGoal()
        {
            CurrentlyCrafing.Remove(this);
        }

        public virtual void PerformGoal(ref NPCBase.NPCState state)
        {
            if (CraftingJobInstance.SelectedRecipe == null)
                GetNextRecipe(ref state);

            CraftingJobInstance.NPC.LookAt(CraftingJobInstance.Position.Vector);
            state.JobIsDone = true;
            state.SetCooldown(0.05, 0.15);

            if (CraftingJobInstance.SelectedRecipe != null)
            {
                if (CraftingJobInstance.SelectedRecipeCount > 0 && CraftingJobInstance.SelectedRecipe.IsPossible(Job.Owner, state.Inventory, RecipeSettingsGroup))
                {
                    state.Inventory.Remove(CraftingJobInstance.SelectedRecipe.Requirements);
                    CraftingResults.Clear();
                    CraftingResults.Add(CraftingJobInstance.SelectedRecipe.Results);
                    ModLoader.Callbacks.OnNPCCraftedRecipe.Invoke(Job, CraftingJobInstance.SelectedRecipe, CraftingResults);
                    float cd = CraftingJobSettings.CraftingCooldown * Pipliz.Random.NextFloat(0.9f, 1.1f);

                    if (CraftingResults.Count > 0)
                    {
                        state.Inventory.Add(CraftingResults);
                        RecipeResult toShow = RecipeResult.GetWeightedRandom(CraftingResults);

                        if (toShow.Amount > 0)
                            state.SetIndicator(new IndicatorState(cd, toShow.Type));
                        else
                            state.SetCooldown(cd);

                        if (CraftingJobSettings.OnCraftedAudio != null)
                            AudioManager.SendAudio(GetPosition().Vector, CraftingJobSettings.OnCraftedAudio);
                    }
                    else
                    {
                        state.SetIndicator(new IndicatorState(cd, NPCIndicatorType.None));
                    }

                    if (!CraftingJobInstance.IsCrafting)
                    {
                        CraftingJobInstance.IsCrafting = true;
                        OnStartCrafting();
                    }

                    state.JobIsDone = false;
                    CraftingJobInstance.SelectedRecipeCount--;
                }
                else
                {
                    CraftingJobInstance.SelectedRecipe = null;

                    state.SetCooldown(0.05, 0.15);

                    if (CraftingJobInstance.IsCrafting)
                    {
                        CraftingJobInstance.IsCrafting = false;
                        OnStopCrafting();
                    }
                }

                return;
            }
        }

        public virtual void GetNextRecipe(ref NPCBase.NPCState state)
        {
            if (NextRecipe.MatchType == Recipe.RecipeMatchType.Invalid)
                NextRecipe = Recipe.MatchRecipe(CraftingJobSettings.GetPossibleRecipes(CraftingJobInstance), Job.Owner, RecipeSettingsGroup);

            switch (NextRecipe.MatchType)
            {
                case Recipe.RecipeMatchType.FoundMissingRequirements:
                case Recipe.RecipeMatchType.AllDone:
                    {
                        if (!state.Inventory.IsEmpty)
                        {
                            JobSettings.SetGoal(Job, new PutItemsInCrateGoal(Job, JobSettings, this, state.Inventory.Inventory));
                            state.Inventory.Inventory.Clear();
                            state.SetCooldown(0.2, 0.4);
                            break;
                        }

                        float cooldown = Pipliz.Random.NextFloat(8f, 16f);
                        if (NextRecipe.MatchType == Recipe.RecipeMatchType.AllDone)
                        {
                            state.SetIndicator(new IndicatorState(cooldown, BuiltinBlocks.Indices.erroridle));
                        }
                        else
                        {
                            GetItemsFromCrate(ref state);
                        }

                        Job.Owner.Stats.RecordNPCIdleSeconds(Job.NPCType, cooldown);
                        break;
                    }
                case Recipe.RecipeMatchType.FoundCraftable:
                    CraftingJobInstance.SelectedRecipe = NextRecipe.FoundRecipe;
                    CraftingJobInstance.SelectedRecipeCount = NextRecipe.FoundRecipeCount;
                    GetItemsFromCrate(ref state);
                    state.SetCooldown(0.2, 0.4);
                    NextRecipe = Recipe.MatchRecipe(CraftingJobSettings.GetPossibleRecipes(CraftingJobInstance), Job.Owner, RecipeSettingsGroup);
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(condition: false, "Unexpected RecipeMatchType: " + NextRecipe.MatchType);
                    break;
            }
        }

        public virtual void GetItemsFromCrate(ref NPCBase.NPCState state)
        {
            state.Inventory.Add(CraftingJobInstance.SelectedRecipe.Requirements);
            JobSettings.SetGoal(Job, new GetItemsFromCrateGoal(Job, JobSettings, this, CraftingJobInstance.SelectedRecipe.Requirements));
        }

        public virtual void OnStopCrafting()
        {

        }

        public virtual void OnStartCrafting()
        {

        }

       
    }
}
