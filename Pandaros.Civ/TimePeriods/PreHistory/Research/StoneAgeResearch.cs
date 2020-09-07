using Pandaros.API;
using Pandaros.API.Research;
using Pandaros.API.Research.Conditions;
using Pandaros.Civ.TimePeriods.BronzeAge.Items;
using Pandaros.Civ.TimePeriods.IronAge.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Pandaros.Civ.TimePeriods.PreHistory.Quests;
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
                    new ColonistCountCondition() { Threshold = 13 },
                    new QuestCompleteGoal(StoneAgeQuest.NAME)
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
                    new RecipeUnlock(Basket.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLINGBULLET.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLING.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(IronChest.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(PaddedCrate.Name, ERecipeUnlockType.Recipe)
                }
            }
        };
    }

}
