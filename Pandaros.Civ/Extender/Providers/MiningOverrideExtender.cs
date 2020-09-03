using ModLoaderInterfaces;
using Pandaros.API.Extender;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pandaros.Civ.WorldGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class MiningOverrideExtender : IOnTryChangeBlockExtender, IAfterWorldLoad
    {
        public List<Type> LoadedAssembalies { get; set; } = new List<Type>();

        public string InterfaceName => nameof(IMineTypeOverride);

        public Type ClassType { get; set; } = null;

        static Dictionary<ushort, Dictionary<string, Dictionary<string, StoredItem>>> callbacks = new Dictionary<ushort, Dictionary<string, Dictionary<string, StoredItem>>>(); 

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.PlayerClickedData != null && tryChangeBlockData.PlayerClickedData.HitType != Shared.PlayerClickedData.EHitType.Block)
                return;

            
        }

        public void AfterWorldLoad()
        {
            foreach (var s in LoadedAssembalies)
            {
                if (Activator.CreateInstance(s) is IMineTypeOverride sb)
                {
                    foreach (var block in sb.BlockNames)
                    {
                        var id = ItemId.GetItemId(block);

                        if (!callbacks.TryGetValue(id, out var holdingTypes))
                        {
                            holdingTypes = new Dictionary<string, Dictionary<string, StoredItem>>();
                            callbacks[id] = holdingTypes;
                        }

                        foreach (var item in sb.Replacement)
                        {
                            if (holdingTypes.TryGetValue(item.Name, out var si))
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}
