using Jobs.Implementations.Construction;
using Pandaros.API;
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
    public class JobReward : IPandaQuestReward
    {
        public static Dictionary<string, string> JobToQuestMappings { get; set; } = new Dictionary<string, string>();

        public Recipes.Recipe Recipe { get; set; }
        public string ItemIconName { get; set; }
        public string RewardKey { get; set; }
        public string NPCType { get; set; }
        public string LocalizationKey { get; set; }
        public LocalizationHelper LocalizationHelper { get; set; }

        public JobReward(string quest, string npcType, string rewardKey, string icon, string localizationKey = null, LocalizationHelper localizationHelper = null)
        {
            NPCType = npcType;
            RewardKey = rewardKey;
            LocalizationHelper = localizationHelper;
            LocalizationKey = localizationKey;
            JobToQuestMappings[npcType] = quest;
            ItemIconName = icon;

            if (LocalizationHelper == null)
                LocalizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "Quests");

            if (string.IsNullOrEmpty(LocalizationKey))
                LocalizationKey = nameof(JobReward);
        }

        public string GetRewardText(IPandaQuest quest, Colony colony, Players.Player player)
        {
            var formatStr = LocalizationHelper.LocalizeOrDefault(LocalizationKey, player);

            if (formatStr.Count(c => c == '{') == 1)
                return string.Format(LocalizationHelper.LocalizeOrDefault(LocalizationKey, player), Localization.GetSentence(player.LastKnownLocale, NPCType));
            else
                return formatStr;
        }

        public void IssueReward(IPandaQuest quest, Colony colony)
        {
            
        }
    }
}
