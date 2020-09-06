using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.Jobs.BaseReplacements;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class WoodWorkerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.WoodWorkerModEntries")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<CraftingJobInstance>(
                    new WoodWorker(),
                    (setting, pos, type, bytedata) => new CraftingJobInstance(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new CraftingJobInstance(setting, pos, type, colony)
                )
            );
        }
    }


    /*public class WoodWorkerTexture : CSTextureMapping
    {
        public override string name => StonePuncher.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "WoodWorker.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "WoodWorker.png");
        //public override string height => GameSetup.Textures.GetPath(TextureType.height, "WoodWorker.png");
    }*/


    public class WoodWorker : PandaCrafingSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(WoodWorker));

        public WoodWorker() : base(Name, Name)
        {
            NPCType.AddSettings(new NPCTypeStandardSettings
            {
                keyName = Name,
                printName = "Wood Worker",
                maskColor1 = new UnityEngine.Color32(51, 51, 77, 255),
                type = NPCTypeID.GetNextID(),
                inventoryCapacity = 500f
            });

            NPCType = NPCType.GetByKeyNameOrDefault(Name);

        }
    }
    public class WoodWorkerJobBlock : CSType
    {
        public override string name => WoodWorker.Name;
        public override string icon => "gamedata/textures/icons/splittingstump.png";
        public override string onPlaceAudio => "woodPlace";
        public override string onRemoveAudio => "woodDeleteHeavy";
        public override string sideall => ColonyBuiltIn.ItemTypes.LOGTEMPERATE;
        public override string sideyp => ColonyBuiltIn.ItemTypes.SPLITTINGSTUMP;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Job,
                "aa",
                nameof(TimePeriod.PreHistory),
                CommonCategories.Wood,
                GameSetup.NAMESPACE
            };
    }

    public class WoodWorkerTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id, 3)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(WoodWorker.Name)
        };

        public bool isOptional => false;

        public string name => WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Name;
    }

    public class WoodWorkerTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id, 3)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(WoodWorker.Name)
        };

        public bool isOptional => false;

        public string name => WoodWorker.Name + ColonyBuiltIn.ItemTypes.LOGTAIGA.Name;
    }
}
