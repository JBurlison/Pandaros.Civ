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
using Pandaros.Civ.Storage;
using Pandaros.API.Questing.BuiltinPrerequisites;
using Pandaros.API;

namespace Pandaros.Civ.TimePeriods.PreHistory.Quests
{
    public class GettingStartedQuest : GenericQuest
    {
        public static string NAME { get; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Quests", nameof(GettingStartedQuest));
        public static LocalizationHelper HELPER { get; } = new LocalizationHelper(GameSetup.NAMESPACE, "TimePeriods.PreHistory.Quests");

        public override string GetQuestTitle(Colony colony, Players.Player player)
        {
            return HELPER.LocalizeOrDefault(nameof(GettingStartedQuest), player);
        }

        public GettingStartedQuest() : 
            base(NAME, NAME + "Text", StockpileBlock.Name, HELPER)
        {
        }
        

        public override Dictionary<string, IPandaQuestObjective> QuestObjectives { get; set; } = new Dictionary<string, IPandaQuestObjective>()
        {
            {
                "stockpileblockplaced",
                new BlockPlacedObjective("stockpileblockplaced", StockpileBlock.Name, 1)
            },
            {
                "foodstored",
                new FoodStoredObjective("foodstored", 20)
            },
            {
                "leafBeds",
                new BlockPlacedObjective("leafBeds", LeafBed.NAME, 5)
            },
            {
                "colonistcount",
                new ColonistCountObjective("colonistcount", 1)
            }
        };
        
        public override List<IPandaQuestReward> QuestRewards { get; set; } = new List<IPandaQuestReward>()
        {
            new TextReward("AdvanceToFoodQuest", ColonyBuiltIn.ItemTypes.BERRY, "AdvanceToFoodQuest", HELPER),
            new JobReward(NAME, PrimitiveBerryForager.Name, "PrimitiveBerryForager", ColonyBuiltIn.ItemTypes.BERRY)
        };
    }
}
