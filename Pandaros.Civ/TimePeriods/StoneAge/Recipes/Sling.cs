using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Recipes
{
    public class SlingTaigaRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLING.Id, 1)
        };



        public string name => ColonyBuiltIn.ItemTypes.SLING.Name + ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name;

        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { WoodWorker.Name };
    }
    public class SlingTemperateRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLING.Id, 1)
        };



        public string name => ColonyBuiltIn.ItemTypes.SLING.Name + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name;

        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { WoodWorker.Name };
    }
}
