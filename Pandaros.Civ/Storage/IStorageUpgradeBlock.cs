using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.API.Models;

namespace Pandaros.Civ.Storage
{
    public interface IStorageUpgradeBlock : ICSType
    {
        Dictionary<string, int> CategoryStorageUpgrades { get; set; }
        Dictionary<string, int> ItemStorageUpgrades { get; set; }
        int GlobalStorageUpgrade { get; set; }
    }
}
