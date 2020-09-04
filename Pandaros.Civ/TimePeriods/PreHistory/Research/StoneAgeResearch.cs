using Pandaros.API;
using Pandaros.API.Research;
using Pandaros.Civ.Research;
using Pandaros.Civ.StoneAge.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Research
{
    public class StoneAgeResearch : PandaResearch
    {
        public override string name => GameSetup.GetNamespace("TimePeriods.PreHistory.Research", nameof(StoneAgeResearch));
        public override string IconDirectory => GameSetup.Textures.ICON_PATH;
        public override bool AddLevelToName => false;
        public override int NumberOfLevels => 1;
        public override int BaseIterationCount => 1;

        public override Dictionary<int, List<IResearchableCondition>> Conditions => new Dictionary<int, List<IResearchableCondition>>()
        {
            { 
                0, 
                new List<IResearchableCondition>()
                {
                    new ColonistCountCondition() { Threshold = 10 },
                    new StockpileSizeGoal(150),
                    new JobsPlacedCondition(SlowPorterFromCrate.Name, 1),
                    new JobsPlacedCondition(SlowPorterToCrate.Name, 1)
                }
            }
        };

        public override Dictionary<int, List<RecipeUnlock>> Unlocks => new Dictionary<int, List<RecipeUnlock>>()
        {
            {
                0,
                new List<RecipeUnlock>()
                {
                    new RecipeUnlock(SturdyCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Basket.Name, ERecipeUnlockType.Recipe)
                }
            }
        };
    }

}
