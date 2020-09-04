using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Recipes
{
    public class SlingRecipePlayer : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name, 2)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLING.Id, 5)
        };

        public bool isOptional => false;

        public string name => ColonyBuiltIn.ItemTypes.SLING.Name;

    }

    public class SlingRecipePlayerTemperate : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name, 2)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLING.Id, 5)
        };

        public bool isOptional => false;

        public string name => ColonyBuiltIn.ItemTypes.SLING.Name;

    }
}
