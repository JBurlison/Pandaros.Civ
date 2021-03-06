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
    public class Stick : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(Stick));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "Stick.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 300;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Essential,
            CommonCategories.Ammo,
            "aa",
            nameof(TimePeriod.PreHistory),
            CommonCategories.Wood,
            GameSetup.NAMESPACE
        };
    }

    public class WoodenSpearRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Stick.NAME, 5)
        };

        public string name => Stick.NAME;

        public CraftPriority defaultPriority => CraftPriority.High;

        public int defaultLimit => 20; // more are needed for guards ammo.

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string> { WoodWorker.Name };
    }
}
