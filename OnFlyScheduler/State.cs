using System;

namespace OnFlyScheduler
{
    /// <summary>
    /// it contains job information passed to timers callback method.
    /// </summary>
    public class State
    {
        public bool IsRunning { get; internal set; }
        public bool IsCancelled { get; internal set; }

        public DateTime? LastRunDate { get; internal set; }
        public DateTime? LastEndDate { get; internal set; }

        public Exception LastExceptionOccured { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State()
        {
            IsRunning = false;
            IsCancelled = false;
        }
    }
}
