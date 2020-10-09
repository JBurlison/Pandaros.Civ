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
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;

namespace Pandaros.Civ.TimePeriods.StoneAge.Recipes
{

    public class PlanksRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(RoughWoodenBoard.NAME, 3),
            new RecipeItem(StoneNail.NAME, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(ColonyBuiltIn.ItemTypes.PLANKS.Id, 3)
        };



        public string name => ColonyBuiltIn.ItemTypes.PLANKS.Name;

        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { WoodWorker.Name };
    }
    }
