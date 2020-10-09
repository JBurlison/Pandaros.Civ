using Pandaros.API.localization;
using Pandaros.API.Questing.BuiltinQuests;
using Pandaros.API.Questing.BuiltinObjectives;
using Pandaros.API.Questing.BuiltinRewards;
using Pandaros.API.Questing.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.Civ.Quests;
using Pandaros.API.Models;
using Pandaros.API;
using Pandaros.API.Questing.BuiltinPrerequisites;
using Pandaros.Civ.TimePeriods.StoneAge.Items;

namespace Pandaros.Civ.TimePeriods.StoneAge.Quests
{
    public class TheWheelQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.StoneAge.Quests", nameof(TheWheelQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.StoneAge.Quests");

        public TheWheelQuest() : 
            base(NAME, NAME + "Text", StoneWheel.NAME, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(TheWheelQuest), player);
        }

        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( UsingStoneQuest.NAME)
        }; 
        //public override bool HideQuest { get; } = true;
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "stonewheelinstockpile",
                new ItemsInStockpileObjective("stonewheelinstockpile", StoneWheel.NAME, 20)
            },
            {
                "forager",
                new JobsTakenObjective("forager", Forager.Name, 3)
            }
        };

        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToTheBronzeAgeQuest", ColonyBuiltIn.ItemTypes.BRONZEINGOT, "AdvanceToTheBronzeAgeQuest", HELPER)
        };
    }
}
