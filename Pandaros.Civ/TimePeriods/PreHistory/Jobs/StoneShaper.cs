using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class StoneShaperModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.StoneShaperModEntries")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<CraftingJobInstance>(
                    new StoneShaper(),
                    (setting, pos, type, bytedata) => new CraftingJobInstance(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new CraftingJobInstance(setting, pos, type, colony)
                )
            );
        }
    }

    public class StoneShaper : PandaCrafingSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(StoneShaper));

        public StoneShaper() : base(Name, Name)
        {
            NPCType.AddSettings(new NPCTypeStandardSettings
            {
                keyName = Name,
                printName = "Stone Shaper",
                maskColor1 = new UnityEngine.Color32(51, 51, 77, 255),
                type = NPCTypeID.GetNextID(),
                inventoryCapacity = 500f
            });

            NPCType = NPCType.GetByKeyNameOrDefault(Name);

        }
    }

    public class StoneShaperTexture : CSTextureMapping
    {
        public override string name => StoneShaper.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StoneShaper.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StoneShaper.png");
        //public override string height => GameSetup.Textures.GetPath(TextureType.height, "StoneShaper.png");
    }

    public class StoneShaperJobBlock : CSType
    {
        public override string name => StoneShaper.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "StoneShaper.png");
        public override string onPlaceAudio => "stonePlace";
        public override string onRemoveAudio => "stoneDelete";
        public override string sideall => ColonyBuiltIn.ItemTypes.STONEBLOCK;
        public override string sideyp => StoneShaper.Name;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Job,
                "ab",
                nameof(TimePeriod.PreHistory),
                CommonCategories.Stone,
                GameSetup.NAMESPACE
            };
    }

    public class StoneShaperRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 16)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StoneShaper.Name)
        };

        public bool isOptional => false;

        public string name => StoneShaper.Name;
    }
}
