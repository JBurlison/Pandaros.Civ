using Pandaros.API.Entities;
using Pandaros.API.localization;
using Pandaros.API.Questing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Quests
{
    public class TimePeriodReward : IPandaQuestReward
    {
        public string ItemIconName { get; set; }
        public string RewardKey { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public string LocalizationKey { get; set; }
        public LocalizationHelper LocalizationHelper { get; set; }

        public TimePeriodReward(TimePeriod period, string itemIconName, string rewardKey, string localizationKey = null, LocalizationHelper localizationHelper = null)
        {
            TimePeriod = period;
            LocalizationKey = localizationKey;
            LocalizationHelper = localizationHelper;
            ItemIconName = itemIconName;
            RewardKey = rewardKey;

            if (LocalizationHelper == null)
                LocalizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "Quests");

            if (string.IsNullOrEmpty(LocalizationKey))
                LocalizationKey = nameof(TimePeriodReward);
        }

        public string GetRewardText(IPandaQuest quest, Colony colony, Players.Player player)
        {
            return LocalizationHelper.LocalizeOrDefault(LocalizationKey, player);
        }

        public void IssueReward(IPandaQuest quest, Colony colony)
        {
            var colonystate = ColonyState.GetColonyState(colony);
            colonystate.Stats[nameof(TimePeriod)] = (double)TimePeriod;
        }
    }
}
