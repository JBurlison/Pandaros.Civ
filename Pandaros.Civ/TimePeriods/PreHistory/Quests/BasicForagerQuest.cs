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
using Pandaros.API;

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class BasicForagerQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(BasicForagerQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public BasicForagerQuest() : 
            base(NAME, NAME + "Text", BasicForaging_questIcon.NAME, HELPER)
        {
        }

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(BasicForagerQuest), player);
        }

        public override List<IPandaQuestPrerequisite> QuestPrerequisites { get; set; } = new List<IPandaQuestPrerequisite>()
        {
           new QuestPrerequisite( GettingStartedQuest.NAME)
        };
        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "PrimitiveBerryForager",
                new JobsTakenObjective("PrimitiveBerryForager", PrimitiveBerryForager.Name, 3, HELPER)
            },
            {
                "PrimitiveWoodForager",
                new JobsTakenObjective("PrimitiveWoodForager", PrimitiveWoodForager.Name, 1, HELPER)
            },
            {
                "PrimitiveRockForager",
                new JobsTakenObjective("PrimitiveRockForager", PrimitiveRockForager.Name, 1, HELPER)
            },
            {
                "colonistcount",
                new ColonistCountObjective("colonistcount", 5)
            }
        };
        
        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToWoodWorkerQuest", WoodWorker.Name, "AdvanceToWoodWorkerQuest", HELPER),
            new RecipeUnlockReward(WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Name, "Woodworker", HELPER),
            new RecipeUnlockReward(WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTAIGA.Name, "Woodworker", HELPER)
        };
    }
}
