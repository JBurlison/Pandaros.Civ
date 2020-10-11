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
    public class BerryBushDestory : IOnTryChangeBlock
    {
        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.PlayerClickedData != null && tryChangeBlockData.PlayerClickedData.HitType != Shared.PlayerClickedData.EHitType.Block)
                return;

            if (tryChangeBlockData.TypeOld.Name == ColonyBuiltIn.ItemTypes.BERRYBUSH)
            {
                tryChangeBlockData.InventoryItemResults.Clear();
                tryChangeBlockData.InventoryItemResults.Add(new InventoryItem(ColonyBuiltIn.ItemTypes.BERRY.Id, Pipliz.Random.Next(30, 51)));
            }
        }
    }
}
