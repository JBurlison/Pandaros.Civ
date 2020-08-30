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

    public class BasicCrate : CSType, ICrate
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "BasicCrate");
        public int MaxCrateStackSize { get; set; } = 300;
        public int MaxNumberOfStacks { get; set; } = 32;

        public BasicCrate()
        {
            name = Name;
            sideall = "planks";
            categories = new List<string>()
            {
                "storage"
            };
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "crate.png");
            mesh = GameSetup.MESH_PATH + "crate.obj";
        }
    }

    public class BasicCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id, 10),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(BasicCrate.Name)
        };

        public bool isOptional => false;

        public string name => BasicCrate.Name;
    }
}
