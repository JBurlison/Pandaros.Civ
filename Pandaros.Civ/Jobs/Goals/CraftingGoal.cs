using BlockTypes;
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
        public List<CraftingGoal> CurrentlyCrafing { get; set; } = new List<CraftingGoal>();

        public CraftingGoal(IPandaJob job, string recipieKey, string onCraftedAudio, float craftingCooldown, uint recipeGroupID)
        {
            Job = job;
            RecipeKey = recipieKey;
            RecipeGroupID = new RecipeSettingsGroup.GroupID(recipeGroupID);

            if (Job.Owner.RecipeData.TryGetRecipeGroup(RecipeGroupID, out var recipeSettingsGroup))
                RecipeSettingsGroup = recipeSettingsGroup;
            else if (Job.Owner.RecipeData.TryGetRecipeGroup(RecipeSettingsGroup.GroupID.Default, out var defaultGroup))
                RecipeSettingsGroup = defaultGroup;

            AvailableRecipes = Job.Owner.RecipeData.GetAvailableRecipes(RecipeKey);
            OnCraftedAudio = onCraftedAudio;
            CraftingCooldown = craftingCooldown;
            CurrentlyCrafing.Add(this);
        }

        public List<RecipeResult> CraftingResults { get; set; } = new List<RecipeResult>();
        public AvailableRecipesEnumerator AvailableRecipes { get; set; }
        public IPandaJob Job { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }
        public string RecipeKey { get; set; }
        public RecipeSettingsGroup.GroupID RecipeGroupID { get; set; }
        public RecipeSettingsGroup RecipeSettingsGroup { get; set; }
        public Recipe CurrentRecipe { get; set; }
        public int CurrentRecipeCount { get; set; }
        public bool IsCrafting { get; set; }
        public float CraftingCooldown { get; set; }
        public string OnCraftedAudio { get; set; }

        public virtual Vector3Int GetPosition()
        {
            return Job.Position;
        }

        public virtual void LeavingGoal()
        {
            CurrentlyCrafing.Remove(this);
        }

        public virtual void PerformGoal(ref NPCBase.NPCState state)
        {
            if (CurrentRecipe == null)
                GetNextRecipe(ref state);

            if (CurrentRecipe != null)
            {
                if (CurrentRecipeCount > 0 && CurrentRecipe.IsPossible(Job.Owner, state.Inventory, RecipeSettingsGroup))
                {
                    state.Inventory.Remove(CurrentRecipe.Requirements);
                    CraftingResults.Clear();
                    CraftingResults.Add(CurrentRecipe.Results);
                    ModLoader.Callbacks.OnNPCCraftedRecipe.Invoke(Job, CurrentRecipe, CraftingResults);
                    float cd = CraftingCooldown * Pipliz.Random.NextFloat(0.9f, 1.1f);

                    if (CraftingResults.Count > 0)
                    {
                        state.Inventory.Add(CraftingResults);
                        RecipeResult toShow = RecipeResult.GetWeightedRandom(CraftingResults);

                        if (toShow.Amount > 0)
                            state.SetIndicator(new IndicatorState(cd, toShow.Type));
                        else
                            state.SetCooldown(cd);

                        if (OnCraftedAudio != null)
                            AudioManager.SendAudio(Job.Position.Vector, OnCraftedAudio);
                    }
                    else
                    {
                        state.SetIndicator(new IndicatorState(cd, NPCIndicatorType.None));
                    }

                    if (!IsCrafting)
                    {
                        IsCrafting = true;
                        OnStartCrafting();
                    }

                    CurrentRecipeCount--;
                }
                else
                {
                    CurrentRecipe = null;

                    if (!state.Inventory.IsEmpty)
                        GetItemsFromCrate(ref state);

                    state.SetCooldown(0.05, 0.15);

                    if (IsCrafting)
                    {
                        IsCrafting = false;
                        OnStopCrafting();
                    }
                }
                return;
            }
        }

        public virtual void GetNextRecipe(ref NPCBase.NPCState state)
        {
            Recipe.RecipeMatch recipeMatch = Recipe.MatchRecipe(AvailableRecipes, Job.Owner, RecipeSettingsGroup);

            switch (recipeMatch.MatchType)
            {
                case Recipe.RecipeMatchType.FoundMissingRequirements:
                case Recipe.RecipeMatchType.AllDone:
                    {
                        if (!state.Inventory.IsEmpty)
                        {
                            //TODO ///state.Inventory.
                            Job.SetGoal(new PutItemsInCrateGoal(Job, this, new Storage.StoredItem[0]));
                            state.SetCooldown(0.2, 0.4);
                            break;
                        }

                        float cooldown = Pipliz.Random.NextFloat(8f, 16f);
                        if (recipeMatch.MatchType == Recipe.RecipeMatchType.AllDone)
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
                    CurrentRecipe = recipeMatch.FoundRecipe;
                    CurrentRecipeCount = recipeMatch.FoundRecipeCount;
                    GetItemsFromCrate(ref state);
                    state.SetCooldown(0.2, 0.4);
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(condition: false, "Unexpected RecipeMatchType: " + recipeMatch.MatchType);
                    break;
            }
        }

        public virtual void GetItemsFromCrate(ref NPCBase.NPCState state)
        {
            state.Inventory.Add(CurrentRecipe.Requirements);
            Job.SetGoal(new GetItemsFromCrateGoal(Job, this, CurrentRecipe.Requirements));
        }

        public virtual void OnStopCrafting()
        {

        }

        public virtual void OnStartCrafting()
        {

        }
    }
}
