using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class SlowPorterModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.SlowPorterModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PorterJob>(
                    new SlowPorter(),
                    (setting, pos, type, bytedata) => new PorterJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PorterJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class SlowPorterSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = SlowPorter.Name;
        public string printName { get; set; } = "Pre-History Porter";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class SlowPorter : PorterJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(SlowPorter));

        public SlowPorter() : base(Name, Name)
        {
            
        }
    }

    public class SlowPorterTexture : CSTextureMapping
    {
        public override string name => SlowPorter.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "MachinistBenchTop.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "MachinistBenchTop.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "MachinistBenchTop.png");
    }

    public class MachinistJobType : CSType
    {
        public override string name => SlowPorter.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "SlowPorter.png");
        public override string onPlaceAudio => "stonePlace";
        public override string onRemoveAudio => "stoneDelete";
        public override string sideall => ColonyBuiltIn.ItemTypes.STONEBRICKS;
        public override string sideyp => SlowPorter.Name;
        public override List<string> categories => new List<string>() { "job", GameSetup.NAMESPACE };
    }

    public class BasketRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id, 10),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SlowPorter.Name)
        };

        public bool isOptional => false;

        public string name => SlowPorter.Name;
    }
}
