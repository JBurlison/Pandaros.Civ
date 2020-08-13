using Chatting;
using Pandaros.API.AI;
using Pandaros.API.ColonyManagement;
using Pandaros.API.Items;
using Pandaros.API.Items.Armor;
using Pandaros.API.Jobs.Roaming;
using Pandaros.API.Monsters;
using Pandaros.API.Server;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace Pandaros.Civ
{
    public class GameSetup
    {
        public const string NAMESPACE = "Pandaros.Civ";
        public static string ICON_PATH = "icons/";
        public static string MESH_PATH = "Meshes/";
        public static string NPC_PATH = "npc/";
        public static string MOD_FOLDER = @"";
        public static string SAVE_LOC = "";
        public static readonly Version MOD_VER = new Version(0, 0, 0, 1);


        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, NAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path).Replace("\\", "/");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            ICON_PATH = Path.Combine(MOD_FOLDER, "icons").Replace("\\", "/") + "/";
            MESH_PATH = Path.Combine(MOD_FOLDER, "Meshes").Replace("\\", "/") + "/";

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
    }
}
