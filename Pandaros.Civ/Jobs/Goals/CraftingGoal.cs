using BlockTypes;
using Jobs;
using NPC;
using Pandaros.API;
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

    public class CraftingRequests : ICrateRequest
    {
        public Dictionary<ushort, StoredItem> GetItemsNeeded(Vector3Int crateLocation)
        {
            var items = new Dictionary<ushort, StoredItem>();

            foreach (var crafter in CraftingGoal.CurrentlyCrafing)
            {
                if (StorageFactory.CrateLocations.TryGetValue(crafter.Job.Owner, out var crateLocs))
                {
                    if (!crateLocs.ContainsKey(crafter.ClosestCrate))
                        crafter.ClosestCrate = crafter.CraftingJobInstance.Position.GetClosestPosition(crateLocs.Keys.ToList());

                    if (crafter.ClosestCrate == crateLocation)
                    {
                        var maxSize = crateLocs[crateLocation].CrateType.MaxCrateStackSize;

                        if (crafter.CraftingJobInstance.SelectedRecipe != null)
                        {
                            items.AddRange(crafter.CraftingJobInstance.SelectedRecipe.Requirements, maxSize);
                        }

                        if (crafter.NextRecipe.FoundRecipe != null)
                        {
                            items.AddRange(crafter.NextRecipe.FoundRecipe.Requirements, maxSize);
                        }
                    }
                }
            }

            return items;
        }
    }
    public class CraftingGoal : INpcGoal
    {
        public static List<CraftingGoal> CurrentlyCrafing { get; set; } = new List<CraftingGoal>();

        public CraftingGoal(IJob job, IPandaJobSettings jobSettings, CraftingJobSettings settings)
        {
            CraftingJobInstance = job as CraftingJobInstance;
            JobSettings = jobSettings;
            CurrentlyCrafing.Add(this);
            Job = job;
            CraftingJobSettings = settings;
            ClosestCrate = CraftingJobInstance.Position.GetClosestPosition(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());
        }

        public Vector3Int ClosestCrate { get; set; }
        public CraftingJobInstance CraftingJobInstance { get; set; }
        public CraftingJobSettings CraftingJobSettings { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public List<RecipeResult> CraftingResults { get; set; } = new List<RecipeResult>();
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(CraftingGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Goals", nameof(CraftingGoal));
        public RecipeSettingsGroup RecipeSettingsGroup { get; set; }
        public Recipe.RecipeMatch NextRecipe { get; set; }

        public virtual Vector3Int GetPosition()
        {
            if (!CraftingJobInstance.IsValid)
            {
                CurrentlyCrafing.Remove(this);
                LeavingJob();
                return Job.Owner.Banners.First().Position;
            }

            return ((BlockJobInstance)Job).Position;
        }

        public virtual void LeavingGoal()
        {
            
        }

        public virtual void SetAsGoal()
        {
            if(!CurrentlyCrafing.Contains(this))
                CurrentlyCrafing.Add(this);
        }

        public virtual void LeavingJob()
        {
            lock (CraftingGoal.CurrentlyCrafing)
                CurrentlyCrafing.Remove(this);
        }

        public virtual void PerformGoal(ref NPCBase.NPCState state)
        {
            CraftingJobInstance.NPC.LookAt(CraftingJobInstance.Position.Vector);
            RecipeSettingsGroup = Job.Owner.RecipeData.GetRecipeGroup(CraftingJobInstance.CraftingGroupID);
            state.JobIsDone = true;
            state.SetCooldown(0.05, 0.15);

            if (CraftingJobInstance.SelectedRecipe != null)
            {
                if (CraftingJobInstance.SelectedRecipeCount > 0 && CraftingJobInstance.SelectedRecipe.IsPossible(Job.Owner, state.Inventory, RecipeSettingsGroup))
                {
                    if (!state.Inventory.TryRemove(CraftingJobInstance.SelectedRecipe.Requirements))
                    {
                        GetItemsFromCrate(ref state);
                    }
                    else
                    {
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
                }
                else
                {
                    CraftingJobInstance.SelectedRecipe = null;
                    CraftingJobInstance.SelectedRecipeCount = 0;

                    state.SetCooldown(0.05, 0.15);
                    StopCrafting();
                }

                return;
            }

            StopCrafting();

            if (!state.Inventory.IsEmpty)
            {
                PutItemsInCrate(ref state);
                return;
            }

            GetNextRecipe(ref state);
        }

        public virtual void StopCrafting()
        {
            if (CraftingJobInstance.IsCrafting)
            {
                CraftingJobInstance.IsCrafting = false;
                OnStopCrafting();
            }
        }

        public virtual void GetNextRecipe(ref NPCBase.NPCState state)
        {
            NextRecipe = Recipe.MatchRecipe(CraftingJobSettings.GetPossibleRecipes(CraftingJobInstance), Job.Owner, RecipeSettingsGroup);

            switch (NextRecipe.MatchType)
            {
                case Recipe.RecipeMatchType.FoundMissingRequirements:
                case Recipe.RecipeMatchType.AllDone:
                    {
                        if (!state.Inventory.IsEmpty)
                        {
                            PutItemsInCrate(ref state);
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

        public virtual void PutItemsInCrate(ref NPCBase.NPCState state)
        {
            JobSettings.SetGoal(Job, new PutItemsInCrateGoal(Job, JobSettings, this, state.Inventory.Inventory.ToList()), ref state);
            state.Inventory.Inventory.Clear();
            state.SetCooldown(0.2, 0.4);
        }

        public virtual void GetItemsFromCrate(ref NPCBase.NPCState state)
        {
            if (CraftingJobInstance.SelectedRecipe != null)
            {
                state.Inventory.Add(CraftingJobInstance.SelectedRecipe.Requirements.ToList());
                JobSettings.SetGoal(Job, new GetItemsFromCrateGoal(Job, JobSettings, this, CraftingJobInstance.SelectedRecipe.Requirements), ref state);
            }
            else if (NextRecipe.MatchType != Recipe.RecipeMatchType.Invalid)
            {
                state.Inventory.Add(NextRecipe.FoundRecipe.Requirements.ToList());
                JobSettings.SetGoal(Job, new GetItemsFromCrateGoal(Job, JobSettings, this, NextRecipe.FoundRecipe.Requirements), ref state);
            }
        }

        public virtual void OnStopCrafting()
        {

        }

        public virtual void OnStartCrafting()
        {

        }

       
    }
}
