using ModLoaderInterfaces;
using Pandaros.API.Extender;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class CrateRequestProvider : IAfterItemTypesDefinedExtender
    {
        public List<Type> LoadedAssembalies { get; set; } = new List<Type>();

        public string InterfaceName => nameof(ICrateRequest);

        public Type ClassType { get; set; } = null;

        public void AfterItemTypesDefined()
        {
            foreach (var s in LoadedAssembalies)
            {
                if (Activator.CreateInstance(s) is ICrateRequest sb)
                {
                    StorageFactory.CrateRequests.Add(sb);
                }
            }
        }
    }
}
