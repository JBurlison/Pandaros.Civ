using ModLoaderInterfaces;
using Pandaros.API;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.PlayerBehavior
{
    public class BerryBushClick : IOnPlayerClicked
    {
        public void OnPlayerClicked(Players.Player player, PlayerClickedData click)
        {
            if (click.ClickType == PlayerClickedData.EClickType.Right &&
                 click.HitType == PlayerClickedData.EHitType.Block &&
                 World.TryGetTypeAt(click.GetVoxelHit().BlockHit, out ushort blockHit) &&
                 blockHit == ColonyBuiltIn.ItemTypes.BERRYBUSH)
            {
                var inv = player.Inventory;
                inv.TryAdd(ColonyBuiltIn.ItemTypes.BERRY, 1);
            }
        }
    }
}
