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
    public class UsingStoneQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.StoneAge.Quests", nameof(UsingStoneQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.StoneAge.Quests");

        public UsingStoneQuest() : 
            base(NAME, NAME + "Text", PreHistory.Items.Rock.NAME, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(UsingStoneQuest), player);
        }
        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( PreHistory.Quests.StoneAgeQuest.NAME),
           new SciencePrerequisite( GameSetup.GetNamespace("TimePeriods.PreHistory.Research", nameof(PreHistory.Research.StoneAgeResearch)))
        }; 
        public override bool HideQuest { get; } = true;
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "stonenailinstockpile",
                new ItemsInStockpileObjective("stonenailinstockpile", StoneNail.NAME, 20)
            },
            {
                "stonepickaxeinstockpile",
                new ItemsInStockpileObjective("stonepickaxeinstockpile", StonePickaxe.NAME, 5)
            },
            {
                "stoneshaper",
                new JobsTakenObjective("stoneshaper", StoneShaper.Name, 1)
            }/*,
            {
                "stoneminer",
                new JobsTakenObjective("stoneminer", StoneMiner.Name, 2)
            }*/
            // need to add stone miner here after
        };

        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToPorterQuest", SlowPorterFromCrate.Name, "AdvanceToPorterQuest", HELPER),
            new TextReward("AdvanceToTheWheelQuest", StoneWheel.NAME, "AdvanceToTheWheelQuest", HELPER)
        };
    }
}
