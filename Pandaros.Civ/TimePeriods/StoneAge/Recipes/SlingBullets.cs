using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.StoneAge.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
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
            new RecipeItem(SharpRock.NAME, 3)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLINGBULLET.Id, 6)
        };

        

        public string name => ColonyBuiltIn.ItemTypes.SLINGBULLET.Name;

        public CraftPriority defaultPriority => CraftPriority.High;
        public int defaultLimit => 15;

        public string Job => StoneShaper.Name;

        public List<string> JobBlock => new List<string>() { StoneShaper.Name };
    }

    /*public class SlingBulletRecipePlayer : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 6)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLINGBULLET.Id, 10)
        };

        

        public string name => ColonyBuiltIn.ItemTypes.SLINGBULLET.Name;

    }*/
}
