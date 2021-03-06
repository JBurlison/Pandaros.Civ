﻿using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class LeafSash : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(LeafSash));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "LeafSash.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 5;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Essential,
            CommonCategories.RecruitmentItem,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class LeafSashTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafSash.NAME)
        };

        public string name => LeafSash.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE;
    }
    public class LeafSashTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafSash.NAME)
        };

        public string name => LeafSash.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA;
    }

    public class LeafSashTemperateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafSash.NAME)
        };

        public string name => LeafSash.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
    public class LeafSashTaigaJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafSash.NAME)
        };

        

        public string name => LeafSash.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}
