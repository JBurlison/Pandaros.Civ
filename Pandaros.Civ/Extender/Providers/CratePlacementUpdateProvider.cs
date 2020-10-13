using ModLoaderInterfaces;
using Pandaros.API.Extender;
using Pandaros.Civ.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class CratePlacementUpdateProvider : IOnTryChangeBlockExtender, IAfterWorldLoad
    {
        public List<Type> LoadedAssembalies { get; set; } = new List<Type>();

        public string InterfaceName => nameof(ICratePlacementUpdate);

        public Type ClassType { get; set; } = null;

        static List<ICratePlacementUpdate> callbacks = new List<ICratePlacementUpdate>();

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.PlayerClickedData != null && tryChangeBlockData.PlayerClickedData.HitType != Shared.PlayerClickedData.EHitType.Block)
                return;

            var colony = tryChangeBlockData?.RequestOrigin.AsPlayer?.ActiveColony;

            if (colony == null)
                colony = tryChangeBlockData.RequestOrigin.AsColony;

            if (colony != null && StorageFactory.CrateTracker.Positions.TryGetValue(tryChangeBlockData.Position, out var crate))
            {
                PlacementEventType placementEventType = PlacementEventType.Placed;

                if (StorageFactory.CrateTypes.ContainsKey(tryChangeBlockData.TypeOld.Name))
                    placementEventType = PlacementEventType.Removed;

                foreach (var s in callbacks)
                    s.CratePlacementUpdate(colony, placementEventType, tryChangeBlockData.Position);
            }
        }

        public void AfterWorldLoad()
        {
            foreach (var s in LoadedAssembalies)
            {
                if (Activator.CreateInstance(s) is ICratePlacementUpdate sb)
                {
                    callbacks.Add(sb);
                }
            }
        }
    }
}
