using Pandaros.API.localization;
using Pandaros.API.Questing.BuiltinQuests;
using Pandaros.API.Questing.BuiltinObjectives;
using Pandaros.API.Questing.BuiltinRewards;
using Pandaros.API.Questing.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.Civ.Quests;
using Pandaros.Civ.Storage;
using Pandaros.API.Questing.BuiltinPrerequisites;
using Pandaros.API;

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class FoodQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(FoodQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public FoodQuest() : 
            base(NAME, NAME + "Text", ColonyBuiltIn.ItemTypes.BERRY, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(FoodQuest), player);
        }

        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( GettingStartedQuest.NAME)
        };
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "PrimitiveBerryForager",
                new JobsTakenObjective("PrimitiveBerryForager", PrimitiveBerryForager.Name, 4, HELPER)
            },
            {
                "foodstored",
                new FoodStoredObjective("foodstored", 25000)
            }
        };
        
        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToWoodWorkerQuest", WoodWorker.Name, "AdvanceToWoodWorkerQuest", HELPER),
            new TextReward("AdvanceToGuardsQuest", Stick.NAME, "AdvanceToGuardsQuest", HELPER),
            new RecipeUnlockReward(WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTAIGA.Name, "WoodworkerTemperate", WoodWorker.Name, HELPER),
            new RecipeUnlockReward(WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Name, "WoodworkerTaiga", WoodWorker.Name, HELPER),
            new JobReward(NAME, PrimitiveWoodForager.Name, "PrimitiveWoodForager", Wood.NAME),
            new JobReward(NAME, PrimitiveRockForager.Name, "PrimitiveRockForager", Rock.NAME),
            new JobReward(NAME, RockThrower.NameDay, "RockThrowerDay", Rock.NAME),
            new JobReward(NAME, RockThrower.NameNight, "RockThrowerNight", Rock.NAME),
            new JobReward(NAME, SpearThrower.NameDay, "SpearThrowerDay", Stick.NAME),
            new JobReward(NAME, SpearThrower.NameNight, "SpearThrowerNight", Stick.NAME)
        };
    }
}
