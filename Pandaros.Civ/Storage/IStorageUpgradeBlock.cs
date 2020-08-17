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
        Dictionary<string, int> CategoryUpgrades { get; set; }
        Dictionary<string, int> ItemUpgrades { get; set; }
        int GlobalUpgrade { get; set; }
    }
}
