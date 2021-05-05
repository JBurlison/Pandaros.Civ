using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Recipes
{
    public class BerryMeal : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.BERRY.Name, 8)
        };

        public List<RecipeResult> results =>  new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.BERRYMEAL.Name)
        };

        public string name => "pipliz.berry.berrymeal.PlayerRecipe";
    }
}
