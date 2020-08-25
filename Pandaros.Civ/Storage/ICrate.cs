using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public interface ICrate
    {
        int MaxStackSize { get; set; }
        int MaxNumberOfStacks { get; set; }
    }
}
