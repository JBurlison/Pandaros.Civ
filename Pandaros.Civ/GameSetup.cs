using Chatting;
using ModLoaderInterfaces;
using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.AI;
using Pandaros.API.ColonyManagement;
using Pandaros.API.Extender;
using Pandaros.API.Items;
using Pandaros.API.Items.Armor;
using Pandaros.API.Jobs.Roaming;
using Pandaros.API.Monsters;
using Pandaros.API.Research;
using Pandaros.API.Server;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pandaros.Civ
{
    [ModLoader.ModManager]
    [LoadPriority(double.MaxValue)]
    public class GameSetup : IOnLoadModJSONFiles
    {
        public const string NAMESPACE = "Pandaros.Civ";
        public static string MESH_PATH = "Meshes/";
        public static string MOD_FOLDER = @"";
        public static string SAVE_LOC = "";
        public static readonly Version MOD_VER = new Version(0, 0, 0, 1);

        public static class Textures
        {
            public const string SELF = "SELF";
            public static string TEXTURE_PATH = "Textures/";
            public static string Albedo = "Textures/albedo";
            public static string Emissive = "Textures/emissiveMaskAlpha";
            public static string Height = "Textures/heightSmoothnessSpecularity";
            public static string Normal = "Textures/normal";
            public static string ICON_PATH = "icons/";
            public static string NPC_PATH = "npc/";

           

            public static string GetPath(TextureType textureType, string textureFileName)
            {
                switch (textureType)
                {
                    case TextureType.aldebo:
                        return Path.Combine(Albedo, textureFileName).Replace("\\", "/");
                    case TextureType.emissive:
                        return Path.Combine(Emissive, textureFileName).Replace("\\", "/");
                    case TextureType.height:
                        return Path.Combine(Height, textureFileName).Replace("\\", "/");
                    case TextureType.icon:
                        return Path.Combine(ICON_PATH, textureFileName).Replace("\\", "/");
                    case TextureType.normal:
                        return Path.Combine(Normal, textureFileName).Replace("\\", "/");
                    case TextureType.npc:
                        return Path.Combine(NPC_PATH, textureFileName).Replace("\\", "/");
                }

                return Path.Combine(TEXTURE_PATH, textureFileName).Replace("\\", "/");
            }
        }


        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, NAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path).Replace("\\", "/");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Textures.ICON_PATH = Path.Combine(MOD_FOLDER, "icons").Replace("\\", "/") + "/";
            MESH_PATH = Path.Combine(MOD_FOLDER, "Meshes").Replace("\\", "/") + "/";
            Textures.TEXTURE_PATH = Path.Combine(MOD_FOLDER, "Textures").Replace("\\", "/") + "/";
            Textures.Albedo = Path.Combine(Textures.TEXTURE_PATH, "albedo").Replace("\\", "/") + "/";
            Textures.Emissive = Path.Combine(Textures.TEXTURE_PATH, "emissiveMaskAlpha").Replace("\\", "/") + "/";
            Textures.Height = Path.Combine(Textures.TEXTURE_PATH, "heightSmoothnessSpecularity").Replace("\\", "/") + "/";
            Textures.Normal = Path.Combine(Textures.TEXTURE_PATH, "normal").Replace("\\", "/") + "/";

            CivLogger.Log("Found mod in {0}", MOD_FOLDER);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            CivLogger.Log(args.Name);
            try
            {

                if (args.Name.Contains("System.Numerics"))
                    return Assembly.LoadFile(MOD_FOLDER + "/System.Numerics.dll");
            }
            catch (Exception ex)
            {
                CivLogger.LogError(ex);
            }

            return null;
        }
        
        public static string GetNamespace(params string[] paths)
        {
            return NAMESPACE + "." + string.Join(".", paths);
        }


        [ModLoader.ModCallback(NAMESPACE + ".OnLoadModJSONFiles")]
        public void OnLoadModJSONFiles(List<ModLoader.LoadModJSONFileContext> contexts)
        {
            //var blacklistTypes = new List<string>()
            //{
            //    //"addOrReplaceNPCRecipes",
            //    //"addScience",
            //    //"addOrReplacePlayerRecipes",
            //    //"addOrReplaceStarterPack",
            //    //"scienceBiomePatches"
            //};

            //List<ModLoader.LoadModJSONFileContext> remove = new List<ModLoader.LoadModJSONFileContext>();

            //foreach (var context in contexts)
            //{
            //    if (context.Mod.name == "Colony Survival")
            //    {
            //        List<JObject> keep = new List<JObject>();

            //        foreach (var json in context.Mod.jsonFiles)
            //        {
            //            var ft = json.Value<string>("fileType");

            //            if (!blacklistTypes.Any(b => b.Equals(ft, StringComparison.InvariantCultureIgnoreCase)))
            //                keep.Add(json);
            //        }

            //        context.Mod.jsonFiles = keep.ToArray();


            //        if (blacklistTypes.Any(b => b.Equals(context.FileType, StringComparison.InvariantCultureIgnoreCase)))
            //            remove.Add(context);
            //    }
            //}

            //foreach (var r in remove)
            //    contexts.Remove(r);
        }
    }
}
