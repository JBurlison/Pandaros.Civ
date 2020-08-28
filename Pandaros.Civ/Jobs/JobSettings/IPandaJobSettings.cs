using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface IPandaJobSettings
    {
        /// <summary>
        ///     The NPC Type name. Can register with <see cref="INPCTypeStandardSettings"/>. This is required.
        /// </summary>
        string keyName { get; set; }

        /// <summary>
        ///     Item required to recrut the job. Can be null.
        /// </summary>
        InventoryItem RecruitmentItem { get; set; }

        /// <summary>
        ///     Item Name of the job block
        /// </summary>
        string JobBlock { get; set; }

        /// <summary>
        ///     When true job sleeps at night, when false job sleeps during the day.
        /// </summary>
        bool SleepNight { get; set; }

        /// <summary>
        ///     Set the starting goal for the job.
        /// </summary>
        INpcGoal StartingGoal { get; set; }

        /// <summary>
        ///  Localization 
        /// </summary>
        string LocalizationKey { get; set; }
    }
}
