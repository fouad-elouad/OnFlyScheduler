using System;
using System.Diagnostics;

namespace OnFlyScheduler.UnitTest.Utilities
{
    public class JobState
    {
        public bool? IsCancelled;
        public bool OnJobEndExecuted;
        public bool OnExceptionExecuted;
        public Exception Exception;


        public void OnJobStart(Job job)
        {
            Trace.WriteLine(DateTime.Now.ToString() + " " + nameof(OnJobStart) + ": " + job.EndUserFriendlyName);
        }

        public void OnJobEnd(Job job)
        {
            Trace.WriteLine(DateTime.Now.ToString() + " " + nameof(OnJobEnd) + ": " + job.EndUserFriendlyName);
            IsCancelled = job.State.IsCancelled;
            OnJobEndExecuted = true;
        }

        public void OnException(Exception exception)
        {
            Trace.WriteLine(DateTime.Now.ToString() + " " + nameof(OnException) + ": " + exception.Message);
            Exception = exception;
            OnExceptionExecuted = true;
        }
    }
}
