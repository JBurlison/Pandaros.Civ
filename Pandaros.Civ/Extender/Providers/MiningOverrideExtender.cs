using ModLoaderInterfaces;
using Pandaros.API;
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
        public static List<Type> Loaded { get; set; } = new List<Type>();
        public List<Type> LoadedAssembalies { get { return Loaded; } set { } }

        public string InterfaceName => nameof(IMineTypeOverride);

        public Type ClassType { get; set; } = null;
        // block broken, holding type, replacement Items
        static Dictionary<string, Dictionary<string, Dictionary<string, StoredItem>>> callbacks = new Dictionary<string, Dictionary<string, Dictionary<string, StoredItem>>>(); 

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.PlayerClickedData != null && tryChangeBlockData.PlayerClickedData.HitType != Shared.PlayerClickedData.EHitType.Block)
                return;

            if (callbacks.TryGetValue(tryChangeBlockData.TypeOld.Name, out var holdingType))
            {
                var holdingItem = ItemId.GetItemId(tryChangeBlockData.PlayerClickedData.TypeSelected).Name;

                if (!holdingType.TryGetValue(holdingItem, out var replacements) && !holdingType.TryGetValue(ColonyBuiltIn.ItemTypes.AIR, out replacements))
                {
                    return;
                }
                else
                {
                    tryChangeBlockData.InventoryItemResults = replacements.Values.Select(s => new InventoryItem(s.Name, s.Amount)).ToList();
                }
            }
        }

        public void AfterWorldLoad()
        {
            foreach (var s in Loaded)
            {
                if (Activator.CreateInstance(s) is IMineTypeOverride sb)
                {
                    foreach (var block in sb.BlockNames)
                    {
                        if (!callbacks.TryGetValue(block, out var holdingTypes))
                        {
                            holdingTypes = new Dictionary<string, Dictionary<string, StoredItem>>();
                            callbacks[block] = holdingTypes;
                        }

                        if (!holdingTypes.TryGetValue(sb.HoldingItemType, out var replacementItems))
                        {
                            replacementItems = new Dictionary<string, StoredItem>();
                            holdingTypes[sb.HoldingItemType] = replacementItems;
                        }

                        foreach (var item in sb.Replacement)
                        {
                            if (string.IsNullOrEmpty(item.Name))
                                item.Name = "air";

                            if (replacementItems.TryGetValue(item.Name, out var si))
                                si.Add(item.Amount);
                            else
                                replacementItems[item.Name] = new StoredItem(item.Name, item.Amount);
                        }
                    }
                }
            }
        }
    }
}
