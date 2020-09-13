using Pandaros.API;
using Pandaros.API.Research;
using Pandaros.API.Research.Conditions;
using Pandaros.Civ.TimePeriods.BronzeAge.Items;
using Pandaros.Civ.TimePeriods.IronAge.Items;
using Pandaros.Civ.TimePeriods.BronzeAge.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
//using Pandaros.Civ.TimePeriods.StoneAge.Quests;
using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.Civ.TimePeriods.IndustrialAge.Items;

namespace Pandaros.Civ.TimePeriods.StoneAge.Research
{
    public class BronzeAgeResearch : PandaResearch
    {
        public override string name => GameSetup.GetNamespace("TimePeriods.StoneAge.Research", nameof(BronzeAgeResearch));
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
                    new ColonistCountCondition() { Threshold = 50 },
                    //new QuestCompleteGoal(BronzeAgeQuest.NAME)
                }
            }
        };

        public override Dictionary<int, List<RecipeUnlock>> Unlocks => new Dictionary<int, List<RecipeUnlock>>()
        {
            {
                0,
                new List<RecipeUnlock>()
                {
                    new RecipeUnlock(IronChest.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(PaddedCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Shelving.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Pallet.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(StorageDrawers.Name, ERecipeUnlockType.Recipe)
                }
            }
        };
    }

}
