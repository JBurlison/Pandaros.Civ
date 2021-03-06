﻿using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
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
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "Rock.png");
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

    
}
