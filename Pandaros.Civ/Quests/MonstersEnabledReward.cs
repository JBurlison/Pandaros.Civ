using Jobs.Implementations.Construction;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.API.Questing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Quests
{
    public class MonstersEnabledReward : IPandaQuestReward
    {
        public string ItemIconName { get; set; }
        public string RewardKey { get; set; }
        public string LocalizationKey { get; set; }
        public LocalizationHelper LocalizationHelper { get; set; }

        public MonstersEnabledReward(string rewardKey, string icon, string localizationKey = null, LocalizationHelper localizationHelper = null)
        {
            RewardKey = rewardKey;
            LocalizationHelper = localizationHelper;
            LocalizationKey = localizationKey;
            ItemIconName = icon;

            if (LocalizationHelper == null)
                LocalizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "Quests");

            if (string.IsNullOrEmpty(LocalizationKey))
                LocalizationKey = nameof(MonstersEnabledReward);
        }

        public string GetRewardText(IPandaQuest quest, Colony colony, Players.Player player)
        {
            return LocalizationHelper.LocalizeOrDefault(LocalizationKey, player);
        }

        public void IssueReward(IPandaQuest quest, Colony colony)
        {
            ColonyState cs = ColonyState.GetColonyState(colony);
            cs.MonstersEnabled = true;
        }
    }
}
