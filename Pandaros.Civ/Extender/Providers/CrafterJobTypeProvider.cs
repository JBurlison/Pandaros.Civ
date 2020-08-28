using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Extender;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.Jobs.JobSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    //[LoadPriority(2)]
    //public class CrafterJobTypeProvider : IAfterItemTypesDefinedExtender
    //{
    //    public List<Type> LoadedAssembalies { get; } = new List<Type>();

    //    public string InterfaceName => nameof(ICraftingJobSettings);

    //    public Type ClassType => null;

    //    public void AfterItemTypesDefined()
    //    {
    //        foreach (var jobExtender in LoadedAssembalies)
    //            if (Activator.CreateInstance(jobExtender) is ICraftingJobSettings jobType &&
    //                !string.IsNullOrEmpty(jobType.JobBlock))
    //            {
    //                PandaJobFactory.JobTypes[jobType.JobBlock] = jobType;
    //            }
    //    }
    //}
}
