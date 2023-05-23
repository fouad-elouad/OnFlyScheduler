using OnFlyScheduler.Abstract;
using OnFlyScheduler.Exceptions;
using System;
using System.Threading;

namespace OnFlyScheduler.SpecificJobs
{
    /// <summary>
    /// SingleRecurrenceJob interface
    /// used for single execution job mechanism.
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Abstract.IJob" />
    public interface ISingleRecurrenceJob : IJob
    {
        /// <summary>
        /// Schedules the job at the specified execution date time.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        /// <param name="isImmediate">if set to <c>true</c> [Execute the job immediately].</param>
        /// <exception cref="JobAlreadyScheduledException"></exception>
        void Schedule(DateTime executionDateTime, bool isImmediate);

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        ISingleRecurrenceJob WithTimeOut(TimeSpan timeOut);

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">Exception CallBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        ISingleRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack);

        /// <summary>
        /// Set  StartCallBack.
        /// </summary>
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        ISingleRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack);

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        ISingleRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack);
    }

    /// <summary>
    /// SingleRecurrenceJob class
    /// used for single execution job mechanism.
    /// this class can not be inherited
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Job" />
    /// <seealso cref="OnFlyScheduler.ISingleRecurrenceJob" />
    public sealed class SingleRecurrenceJob : Job, ISingleRecurrenceJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        public SingleRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName)
            : base(callBackMethod, endUserFriendlyName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        /// <param name="logger">the logger.</param>
        public SingleRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName, ILogger logger)
            : base(callBackMethod, endUserFriendlyName, logger)
        {
        }

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        public new ISingleRecurrenceJob WithTimeOut(TimeSpan timeOut)
        {
            return (ISingleRecurrenceJob) base.WithTimeOut(timeOut);
        }

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">Exception CallBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        public new ISingleRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack)
        {
            return (ISingleRecurrenceJob) base.WithOnExceptionCallBack(onExceptionCallBack);
        }

        /// <summary>
        /// Set  StartCallBack.
        /// </summary>
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        public new ISingleRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack)
        {
            return (ISingleRecurrenceJob) base.WithOnStartCallBack(onStartCallBack);
        }

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        public new ISingleRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack)
        {
            return (ISingleRecurrenceJob) base.WithOnEndCallBack(onEndCallBack);
        }

        /// <summary>
        /// Schedules the job at the specified execution date time.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        /// <param name="isImmediate">if set to <c>true</c> [Execute the job immediately].</param>
        /// <exception cref="JobAlreadyScheduledException"></exception>
        public void Schedule(DateTime executionDateTime, bool isImmediate)
        {
            try
            {
                if (this.IsScheduled)
                    throw new JobAlreadyScheduledException();

                this.DueTime = DueTimeFrom(executionDateTime, isImmediate);
                this.FirstExecution = DateTime.Now.Add(DueTime);
                this.Period = new TimeSpan(-1);

                this.Timer = new Timer(TimerCallback, State, DueTime, Period);
                this.IsScheduled = true;
                this.IsSingleRecurrence = true;
                JobManager.TryAdd(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets timer DueTime from executionDateTime.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        /// <param name="isImmediate">if set to <c>true</c> [is immediate].</param>
        /// <returns>DueTime</returns>
        private TimeSpan DueTimeFrom(DateTime executionDateTime, bool isImmediate)
        {
            DateTime now = DateTime.Now;
            TimeSpan dueTime = new TimeSpan(0);
            if (!isImmediate && executionDateTime > now)
            {
                dueTime = executionDateTime - now;
            }
            return dueTime;
        }
    }
}