using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public interface ICrate : ICSType
    {
        int MaxCrateStackSize { get; set; }
        int MaxNumberOfStacks { get; set; }
    }
}
