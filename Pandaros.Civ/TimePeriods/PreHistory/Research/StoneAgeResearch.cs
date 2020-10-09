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
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;

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
                    new ColonistCountCondition() { Threshold = 15 },
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
                    new RecipeUnlock(WovenBasket.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(StoneSpear.NAME, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(SturdyCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(LeafRope.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(LeafRope.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(StonePickaxe.NAME, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(WoodenCart.NAME, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLING.Name + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLING.Name + ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.PLANKS.Name, ERecipeUnlockType.Recipe),

                    //new RecipeUnlock(Forager.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Forager.Name + "player", ERecipeUnlockType.Recipe),
                    new RecipeUnlock(SlowPorterFromCrate.Name + "player", ERecipeUnlockType.Recipe),
                    new RecipeUnlock(SlowPorterFromCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(SlowPorterToCrate.Name + "player", ERecipeUnlockType.Recipe),
                    new RecipeUnlock(SlowPorterToCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(StoneShaper.Name, ERecipeUnlockType.Recipe),

                    new RecipeUnlock(StoneShaper.Name, ERecipeUnlockType.RecipeGroup)
                    /*new RecipeUnlock(SturdyCrate.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Basket.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLINGBULLET.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(ColonyBuiltIn.ItemTypes.SLING.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(StoneShaper.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Forager.Name, ERecipeUnlockType.Recipe),
                    new RecipeUnlock(Forager.Name + "player", ERecipeUnlockType.Recipe)*/
                }
            }
        };
    }

}
