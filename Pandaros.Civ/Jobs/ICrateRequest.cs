using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface ICrateRequest
    {
        Dictionary<ushort, StoredItem> GetItemsNeeded(Vector3Int crateLocation);
    }
}
