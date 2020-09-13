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
using Pandaros.API.Questing.BuiltinPrerequisites;

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class GettingStartedQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(GettingStartedQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public GettingStartedQuest() : 
            base(NAME, NAME + "Text", StoneAgeIcon.NAME, HELPER)
        {
        }

        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "stockpilesize",
                new StockpileSizeObjective("stockpilesize", 100)
            }
        };

        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AbleToCompleteStoneAgeQuest", StoneAgeIcon.NAME, "AbleToCompleteStoneAgeQuest", HELPER),
            new TimePeriodReward(TimePeriod.StoneAge, StoneAgeIcon.NAME, "TimePeriodRewardStoneAge")
        };
    }
}
