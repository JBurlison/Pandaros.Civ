using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;

namespace Pandaros.Civ.TimePeriods.StoneAge.Recipes
{

    public class RockRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(StoneBrick.NAME, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Rock.NAME, 5)
        };

        public string name => Rock.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => StoneShaper.Name;

        public List<string> JobBlock => new List<string> { StoneShaper.Name };
    }
}
