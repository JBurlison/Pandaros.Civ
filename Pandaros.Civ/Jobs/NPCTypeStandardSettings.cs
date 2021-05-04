using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.Jobs
{
    public interface INPCTypeStandardSettings
    {
        string keyName { get; set; }

        float inventoryCapacity { get; set; }

        float movementSpeed { get; set; }

        Color32 maskColor1 { get; set; }
        Color32 maskColor0 { get; set; }
    }
}
