using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public abstract class StorageUpgradeBlockBase : CSType, IStorageUpgradeBlock
    {
        public virtual Dictionary<string, int> CategoryUpgrades { get; set; } = new Dictionary<string, int>();
        public virtual Dictionary<string, int> ItemUpgrades { get; set; } = new Dictionary<string, int>();
        public virtual int GlobalUpgrade { get; set; }
    }
}
