using Microsoft.SqlServer.Server;
using Pandaros.Civ.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.WorldGen
{
    public interface IMineTypeOverride
    {
        string[] BlockNames { get; }
        string HoldingItemType { get; }
        StoredItem[] Replacement { get; }
    }
}
