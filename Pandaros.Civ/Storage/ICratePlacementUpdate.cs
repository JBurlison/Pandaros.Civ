using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public interface ICratePlacementUpdate
    {
        void CratePlacementUpdate(Colony colony, PlacementEventType eventType, Vector3Int position);
    }
}
