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

    public class PlanksRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 3)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.SLINGBULLET.Id, 5)
        };



        public string name => ColonyBuiltIn.ItemTypes.SLINGBULLET.Name;

        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 20;

        public string Job => StoneShaper.Name;

        public List<string> JobBlock => new List<string>() { StoneShaper.Name };
    }
    }
