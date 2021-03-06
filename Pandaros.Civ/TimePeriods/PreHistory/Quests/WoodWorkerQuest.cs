﻿using Pandaros.API.localization;
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
using Jobs;
using Pandaros.API;

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class WoodWorkerQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(WoodWorkerQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public WoodWorkerQuest() : 
            base(NAME, NAME + "Text", WoodWorker.Name, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(WoodWorkerQuest), player);
        }

        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( FoodQuest.NAME)
        };
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "PrimitiveWoodForager",
                new JobsTakenObjective("PrimitiveWoodForager", PrimitiveWoodForager.Name, 1, HELPER)
            },
            {
                "PrimitiveRockForager",
                new JobsTakenObjective("PrimitiveRockForager", PrimitiveRockForager.Name, 1, HELPER)
            },
            {
                "woodworker",
                new JobsTakenObjective("woodworker", WoodWorker.Name, 1, HELPER)
            },
            {
                "woodinstockpile",
                new ItemsInStockpileObjective("woodinstockpile", Wood.NAME, 20, HELPER)
            },
            {
                "stockpileUpgrades",
                new CraftObjective("stockpileUpgrades", HollowedLog.Name, 5)
            },
            {
                "stockpilesize",
                new StockpileSizeObjective("stockpilesize", 140)
            }
        };
        
        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToStoneAgeQuest", StoneAgeIcon.NAME, "AdvanceToStoneAgeQuest", HELPER)
        };
    }
}
