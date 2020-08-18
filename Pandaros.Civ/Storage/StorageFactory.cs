using Pandaros.API;
using Pandaros.API.Extender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class StorageFactory : IOnTimedUpdate, IOnTryChangeBlock
    {
        public double NextUpdateTimeMin => 1;

        public double NextUpdateTimeMax => 4;

        public double NextUpdateTime { get; set; }


        public void OnTimedUpdate()
        {
            
        }

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            
        }
    }
}
