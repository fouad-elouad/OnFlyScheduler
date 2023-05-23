using System;
using System.Collections.Generic;
using System.Linq;

namespace OnFlyScheduler
{
    /// <summary>
    /// static class that contains all scheduled jobs with their states in real time
    /// </summary>
    public static class JobManager
    {
        private static readonly IList<Job> SchedulerList;

        /// <summary>
        /// Initializes the <see cref="JobManager"/> class.
        /// </summary>
        static JobManager()
        {
            SchedulerList = new List<Job>();
        }

        /// <summary>
        /// Adds a Job to the Scheduler List (if the list already contains the job return false)
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <returns>
        /// Add operation state
        /// </returns>
        internal static bool TryAdd(Job job)
        {
            if (!SchedulerList.Contains(job))
            {
                SchedulerList.Add(job);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the specified job from Scheduler List.
        /// </summary>
        /// <param name="job">The job to remove.</param>
        /// <returns>remove operation state</returns>
        internal static bool Remove(Job job)
        {
            return SchedulerList.Remove(job);
        }

        /// <summary>
        /// Gets all scheduled jobs.
        /// </summary>
        /// <returns>All sheduled jobs</returns>
        public static IReadOnlyList<Job> GetAll()
        {
            return (IReadOnlyList<Job>)SchedulerList;
        }

        /// <summary>
        /// Gets jobs based on where.
        /// </summary>
        /// <returns>job list</returns>
        public static IReadOnlyList<Job> Get(Func<Job, bool> where)
        {
            return SchedulerList.Where(where).ToList();
        }

        /// <summary>
        /// Gets job by name.
        /// </summary>
        /// <param name="endUserFriendlyName">Friendly Name.</param>
        /// <returns>Job</returns>
        public static Job GetByFriendlyName(string endUserFriendlyName)
        {
            return SchedulerList.FirstOrDefault(j => j.EndUserFriendlyName == endUserFriendlyName);
        }
    }
}
