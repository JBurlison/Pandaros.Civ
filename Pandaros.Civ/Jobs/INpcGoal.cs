using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface INpcGoal
    {
        string Name { get; set; }
        string LocalizationKey { get; set; }
        Vector3Int Location { get; set; }
    }
}
