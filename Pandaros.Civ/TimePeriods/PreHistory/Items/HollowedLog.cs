using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class HollowedLogTextureTop : CSTextureMapping
    {
        public override string name => HollowedLog.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "HollowedLogTop.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "HollowedLogTop.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "HollowedLogTop.png");
    }

    public class HollowedLog : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(HollowedLog));
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public HollowedLog()
        {
            name = Name;
            GlobalStorageUpgrade = 8;
            CategoryStorageUpgrades.Add("food", 8);
            sideall = Name;
            sideyp = ColonyBuiltIn.ItemTypes.LOGTAIGA;
            sideyn = ColonyBuiltIn.ItemTypes.LOGTAIGA;
            categories = new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.StockpileUpgrade,
                nameof(TimePeriod.PreHistory),
                GameSetup.NAMESPACE
            };
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "HollowedLog.png");
        }
    }

    public class HollowedLogTaigaWoodWorkerRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(HollowedLog.Name)
        };

        

        public string name => HollowedLog.Name + + ColonyBuiltIn.ItemTypes.LOGTAIGA + TimePeriods.PreHistory.Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.High;
        public int defaultLimit => 10; // more are needed for the quest objective.

        public string Job => TimePeriods.PreHistory.Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { TimePeriods.PreHistory.Jobs.WoodWorker.Name };
    }

    public class HollowedLogTemperateWoodWorkerRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(HollowedLog.Name)
        };

        

        public string name => HollowedLog.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE + TimePeriods.PreHistory.Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.High;
        public int defaultLimit => 10; // more are needed for the quest objective.

        public string Job => TimePeriods.PreHistory.Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { TimePeriods.PreHistory.Jobs.WoodWorker.Name };
    }
}
