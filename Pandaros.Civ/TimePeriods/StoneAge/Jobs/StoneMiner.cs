using Jobs.Implementations;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs.BaseReplacements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipliz;
using Pandaros.Civ.Jobs;
using Jobs;
using Pandaros.Civ.TimePeriods.StoneAge.Items;

namespace Pandaros.Civ.TimePeriods.StoneAge.Jobs
{
    [ModLoader.ModManager]
    public static class StoneMinerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.StoneAge.Jobs.StoneMinerModEntries")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<MinerJobInstance>(
                    new StoneMiner(),
                    (setting, pos, type, bytedata) => new MinerJobInstance(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new MinerJobInstance(setting, pos, type, colony)
                )
            );
        }
    }

    public class StoneMiner : PandaMiningJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.StoneAge.Jobs", nameof(StoneMiner));
       
        public StoneMiner() : base(Name, Name, 5, StonePickaxe.NAME, 15, new HashSet<string>() { "darkstone" })
        {
            
        }
    }

    public class StoneMinerNpcSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = StoneMiner.Name;
        public string printName { get; set; } = "StoneMiner";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 2.3f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(122, 114, 86, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }

    public class StoneMinerJobType : CSGenerateType
    {
        public override string typeName => StoneMiner.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#00FF00";
    }
}
