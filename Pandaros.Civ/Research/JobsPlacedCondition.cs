using Pandaros.API.ColonyManagement;
using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Research
{
    public class JobsPlacedCondition : IResearchableCondition
    {
        public int JobCount { get; set; }
        public string JobName { get; set; }

        public JobsPlacedCondition(string jobName, int jobCount)
        {
            JobName = jobName;
            JobCount = jobCount;
        }

        public bool IsConditionMet(AbstractResearchable researchable, ColonyScienceState manager)
        {
            var jobs = ColonyTool.GetJobCounts(manager.Colony);

            return jobs.TryGetValue(JobName, out var counts) && counts.TakenCount >= JobCount;

        }
    }
}
