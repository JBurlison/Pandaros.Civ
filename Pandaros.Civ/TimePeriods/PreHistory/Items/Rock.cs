﻿using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class Rock : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(Rock));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "Rock.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 300;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Stone,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class RockRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.STONEBRICKS.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Rock.NAME, 5)
        };

        public bool isOptional => false;

        public string name => Rock.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => StonePuncher.Name;

        public string JobBlock => StonePuncher.Name;
    }
}