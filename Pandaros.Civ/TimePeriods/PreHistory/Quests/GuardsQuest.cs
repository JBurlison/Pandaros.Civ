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

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class GuardsQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(GuardsQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public GuardsQuest() : 
            base(NAME, NAME + "Text", Stick.NAME, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(GuardsQuest), player);
        }

        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( FoodQuest.NAME)
        };
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "stonethrower",
                new JobsTakenObjective("stonethrower", RockThrower.NameNight, 1, HELPER)
            },
            {
                "spearthrower",
                new JobsTakenObjective("spearthrower", SpearThrower.NameNight, 1, HELPER)
            },
            {
                "rocksinstockpile",
                new ItemsInStockpileObjective("rocksinstockpile", Rock.NAME, 25, HELPER)
            },
            {
                "spearinstockpile",
                new ItemsInStockpileObjective("spearinstockpile", Stick.NAME, 25, HELPER)
            }
        };
        
        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToStoneAgeQuest", StoneAgeIcon.NAME, "AdvanceToStoneAgeQuest", HELPER),
            new MonstersEnabledReward("EnableColonyMonsters", "npcicon")
        };
    }
}
