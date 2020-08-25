﻿using ModLoaderInterfaces;
using Pandaros.API.Extender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class CrateLoader : IAfterItemTypesDefinedExtender
    {
        public List<Type> LoadedAssembalies { get; set; } = new List<Type>();

        public string InterfaceName => nameof(ICrate);

        public Type ClassType { get; set; } = null;

        public void AfterItemTypesDefined()
        {
            foreach (var s in LoadedAssembalies)
            {
                if (Activator.CreateInstance(s) is ICrate sb && !string.IsNullOrEmpty(sb.name))
                {
                    StorageFactory.CrateTypes[sb.name] = sb;
                }
            }
        }
    }
}
