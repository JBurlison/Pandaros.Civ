using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnTryChangeBlock
    {
        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData data)
        {
            var colony = data?.RequestOrigin.AsPlayer?.ActiveColony;

            if (data.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                colony != null)
            {

            }
        }
    }
}
