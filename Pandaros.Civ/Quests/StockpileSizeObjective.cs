using ModLoaderInterfaces;
using Newtonsoft.Json.Linq;
using Pandaros.API.ColonyManagement;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.API.Questing.Models;
using Pandaros.Civ;
using Pandaros.Civ.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Quests
{
    public class StockpileSizeObjective : IPandaQuestObjective
    {
        public string ObjectiveKey { get; set; }
        public float GoalCount { get; set; }
        public string LocalizationKey { get; set; }
        public LocalizationHelper LocalizationHelper { get; set; }

        public StockpileSizeObjective(string objectiveKey, int goalCount, LocalizationHelper localizationHelper = null, string localizationKey = null)
        {
            LocalizationHelper = localizationHelper;
            LocalizationKey = localizationKey;
            ObjectiveKey = objectiveKey;
            GoalCount = goalCount;

            if (LocalizationHelper == null)
                LocalizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "Quests");

            if (string.IsNullOrEmpty(LocalizationKey))
                LocalizationKey = nameof(StockpileSizeObjective);
        }

        public string GetObjectiveProgressText(IPandaQuest quest, Colony colony, Players.Player player)
        {
            var formatStr = LocalizationHelper.LocalizeOrDefault(LocalizationKey, player);
            
            if (formatStr.Count(c => c == '{') == 2)
                return string.Format(LocalizationHelper.LocalizeOrDefault(LocalizationKey, player), StorageFactory.DefaultMax[colony], GoalCount);
            else
                return formatStr;
        }

        public float GetProgress(IPandaQuest quest, Colony colony)
        {
            if (GoalCount == 0)
                return 1;

            if (!StorageFactory.DefaultMax.TryGetValue(colony, out var curMax) || curMax == 0)
                return 0;

            return curMax / GoalCount;
        }

        public void Load(JObject node, IPandaQuest quest, Colony colony)
        {

        }

        public JObject Save(IPandaQuest quest, Colony colony)
        {
            return null;
        }
    }
}
