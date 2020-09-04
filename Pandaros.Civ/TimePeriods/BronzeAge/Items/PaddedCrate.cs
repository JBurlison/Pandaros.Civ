using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.BronzeAge.Items
{

    public class PaddedCrate : CSType, ICrate
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "PaddedCrate");
        public int MaxCrateStackSize { get; set; } = 300;
        public int MaxNumberOfStacks { get; set; } = 15;

        public PaddedCrate()
        {
            name = Name;
            sideall = "coatedplanks";
            categories = new List<string>()
            {
                "essential",
                "storage",
                "crate",
                "ca",
                "bronzeage",
                GameSetup.NAMESPACE
            };
            onPlaceAudio = "woodPlace";
            onRemoveAudio = "woodDeleteHeavy";
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "PaddedCrate.png");
            mesh = GameSetup.MESH_PATH + "SturdyCrate.ply";
        }
    }

    public class PaddedCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.COATEDPLANKS.Id, 6),
            new RecipeItem(ColonyBuiltIn.ItemTypes.COPPERPARTS.Id, 3),
            new RecipeItem(StoneAge.Items.SturdyCrate.Name)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(PaddedCrate.Name)
        };

        public bool isOptional => false;

        public string name => PaddedCrate.Name;
    }
}
