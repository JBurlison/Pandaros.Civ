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
    public static class StonePuncherModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.StonePuncherModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<CraftingJobInstance>(
                    new StonePuncher(),
                    (setting, pos, type, bytedata) => new CraftingJobInstance(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new CraftingJobInstance(setting, pos, type, colony)
                )
            );
        }
    }

    public class StonePuncher : PandaCrafingSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(StonePuncher));

        public StonePuncher() : base(Name, Name)
        {
            NPCType.AddSettings(new NPCTypeStandardSettings
            {
                keyName = Name,
                printName = "Stone Puncher",
                maskColor1 = new UnityEngine.Color32(51, 51, 77, 255),
                type = NPCTypeID.GetNextID(),
                inventoryCapacity = 500f
            });

            NPCType = NPCType.GetByKeyNameOrDefault(Name);

        }
    }

    public class StonePuncherTexture : CSTextureMapping
    {
        public override string name => StonePuncher.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StonePuncher.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StonePuncher.png");
        //public override string height => GameSetup.Textures.GetPath(TextureType.height, "StonePuncher.png");
    }

    public class StonePuncherJobBlock : CSType
    {
        public override string name => StonePuncher.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "StonePuncher.png");
        public override string onPlaceAudio => "stonePlace";
        public override string onRemoveAudio => "stoneDelete";
        public override string sideall => ColonyBuiltIn.ItemTypes.STONEBLOCK;
        public override string sideyp => StonePuncher.Name;
        public override List<string> categories => new List<string>() { "job", GameSetup.NAMESPACE };
    }

    public class StonePuncherRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.STONEBLOCK.Id, 6)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StonePuncher.Name)
        };

        public bool isOptional => false;

        public string name => StonePuncher.Name;
    }
}
