using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Extender;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class NPCTypeStandardSettingProvider : IAfterItemTypesDefinedExtender
    {
        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName => nameof(INPCTypeStandardSettings);

        public Type ClassType => null;

        public void AfterItemTypesDefined()
        {
            var defaults = new NPCTypeStandardSettings();

            foreach (var jobExtender in LoadedAssembalies)
                if (Activator.CreateInstance(jobExtender) is INPCTypeStandardSettings settings &&
                    !string.IsNullOrEmpty(settings.keyName))
                {
                    NPCType.AddSettings(new NPCTypeStandardSettings
                    {
                        keyName = settings.keyName,
                        maskColor1 = settings.maskColor1,
                        maskColor0 = settings.maskColor0,
                        Type = NPCTypeID.GetID(settings.keyName),
                        inventoryCapacity = settings.inventoryCapacity == 0 ? defaults.inventoryCapacity : settings.inventoryCapacity,
                        movementSpeed = settings.movementSpeed == 0 ? defaults.movementSpeed : settings.movementSpeed,
                    });
                }
        }
    }
}
