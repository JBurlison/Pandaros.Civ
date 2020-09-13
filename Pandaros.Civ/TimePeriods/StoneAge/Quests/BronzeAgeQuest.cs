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
using Pandaros.API.Models;
using Pandaros.API;
using Pandaros.API.Questing.BuiltinPrerequisites;

namespace Pandaros.Civ.TimePeriods.StoneAge.Quests
{
    public class BronzeAgeQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.StoneAge.Quests", nameof(BronzeAgeQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.StoneAge.Quests");

        public BronzeAgeQuest() : 
            base(NAME, NAME + "Text", ColonyBuiltIn.ItemTypes.BRONZEINGOT, HELPER)
        {
        }
        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( PreHistory.Quests.StoneAgeQuest.NAME)
        }; 
        public override bool HideQuest { get; } = true;
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "porterfrom",
                new JobsTakenObjective("porterfrom", SlowPorterFromCrate.Name, 1)
            },
            {
                "porterto",
                new JobsTakenObjective("porterfrom", SlowPorterToCrate.Name, 1)
            },
            {
                "colonistcount",
                new ColonistCountObjective("colonistcount", 50)
            },
            {
                "stockpilesize",
                new StockpileSizeObjective("stockpilesize", 250)
            }
        };

        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AbleToCompleteBronzeAgeQuest", ColonyBuiltIn.ItemTypes.BRONZEINGOT, "AbleToCompleteBronzeAgeQuest", HELPER),
            new TimePeriodReward(TimePeriod.BronzeAge, ColonyBuiltIn.ItemTypes.BRONZEINGOT, "TimePeriodRewardBronzeAge")
        };
    }
}
