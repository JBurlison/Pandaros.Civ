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

namespace Pandaros.Civ.PreHistory.Items
{
    public class CarvedLogTextureTop : CSTextureMapping
    {
        public override string name => CarvedLog.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "CarvedLogTop.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "CarvedLogTop.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "CarvedLogTop.png");
    }

    public class CarvedLog : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "CarvedLog");
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public CarvedLog()
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
                CommonCategories.HighPriority,
                CommonCategories.StockpileUpgrade,
                nameof(TimePeriod.PreHistory),
                GameSetup.NAMESPACE
            };
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "CarvedLog.png");
        }
    }

    public class CarvedLogTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(CarvedLog.Name)
        };

        public bool isOptional => false;

        public string name => CarvedLog.Name + ColonyBuiltIn.ItemTypes.LOGTAIGA;
    }
    public class CarvedLogTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(CarvedLog.Name)
        };

        public bool isOptional => false;

        public string name => CarvedLog.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE;
    }
    public class CarvedLogTaigaWoodWorkerRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(CarvedLog.Name)
        };

        public bool isOptional => false;

        public string name => CarvedLog.Name + + ColonyBuiltIn.ItemTypes.LOGTAIGA + TimePeriods.PreHistory.Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Low;
        public int defaultLimit => 5;

        public string Job => TimePeriods.PreHistory.Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { TimePeriods.PreHistory.Jobs.WoodWorker.Name };
    }

    public class CarvedLogTemperateWoodWorkerRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(CarvedLog.Name)
        };

        public bool isOptional => false;

        public string name => CarvedLog.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE + TimePeriods.PreHistory.Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Low;
        public int defaultLimit => 5;

        public string Job => TimePeriods.PreHistory.Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { TimePeriods.PreHistory.Jobs.WoodWorker.Name };
    }
}
