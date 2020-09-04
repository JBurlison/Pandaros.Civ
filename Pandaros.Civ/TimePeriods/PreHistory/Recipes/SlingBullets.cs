using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Recipes
{
    public class SlingBulletRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLINGBULLET.Id, 5)
        };

        public bool isOptional => false;

        public string name => ColonyBuiltIn.ItemTypes.SLINGBULLET.Name;

        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 50;

        public string Job => StonePuncher.Name;

        public string JobBlock => StonePuncher.Name;
    }

    public class SlingBulletRecipePlayer : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLINGBULLET.Id, 5)
        };

        public bool isOptional => false;

        public string name => ColonyBuiltIn.ItemTypes.SLINGBULLET.Name;

    }
}
