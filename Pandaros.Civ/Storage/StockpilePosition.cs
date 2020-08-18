using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class StockpilePosition
    {
        public Vector3Int Position { get; set; }
        public Vector3Int Min { get; set; }
        public Vector3Int Max { get; set; }
    }
}
