using OnFlyScheduler.Abstract;
using System;
using System.Threading;


namespace OnFlyScheduler.SpecificJobs
{
    /// <summary>
    /// CustomRecurrenceJob interface
    /// used for Custom execution job mechanism.
    /// </summary>
    /// <seealso cref="OnFlyScheduler.IJob" />
    public interface ICustomRecurrenceJob : IJob
    {
        /// <summary>
        /// Schedules the job with the specified due time.
        /// </summary>
        /// <param name="dueTime">The due time.</param>
        /// <param name="period">The period.</param>
        void Schedule(TimeSpan dueTime, TimeSpan period);

        /// <summary>
        /// Schedules the job with the specified due time.
        /// </summary>
        /// <param name="executionDateTime">The execution date time</param>
        /// <param name="period">The period</param>
        void Schedule(DateTime executionDateTime, TimeSpan period);

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        ICustomRecurrenceJob WithTimeOut(TimeSpan timeOut);

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">Exception CallBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        ICustomRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack);

        /// <summary>
        /// Set  StartCallBack.
        /// </summary>
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        ICustomRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack);

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        ICustomRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack);

        /// <summary>
        /// Set Schedule New Single Recurrence Job
        /// </summary>
        /// <param name="executionDateTime">The execution date time. </param>
        /// <param name="isImmediate">True if Immediate else false</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        ISingleRecurrenceJob ScheduleNewSingleRecurrenceJob(DateTime executionDateTime, bool isImmediate);
    }

    /// <summary>
    /// CustomRecurrenceJob class
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Job" />
    /// <seealso cref="OnFlyScheduler.ICustomRecurrenceJob" />
    public class CustomRecurrenceJob : Job, ICustomRecurrenceJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        public CustomRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName)
                    : base(callBackMethod, endUserFriendlyName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        /// <param name="logger">the logger.</param>
        public CustomRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName, ILogger logger)
            : base(callBackMethod, endUserFriendlyName, logger)
        {
        }

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns></returns>
        public new ICustomRecurrenceJob WithTimeOut(TimeSpan timeOut)
        {
            return (ICustomRecurrenceJob)base.WithTimeOut(timeOut);
        }

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">oException CallBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        public new ICustomRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack)
        {
            return (ICustomRecurrenceJob)base.WithOnExceptionCallBack(onExceptionCallBack);
        }

        /// </summary>
        /// Set  StartCallBack.
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        public new ICustomRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack)
        {
            return (ICustomRecurrenceJob)base.WithOnStartCallBack(onStartCallBack);
        }

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        public new ICustomRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack)
        {
            return (ICustomRecurrenceJob)base.WithOnEndCallBack(onEndCallBack);
        }

        /// <summary>
        /// Set Schedule New Single Recurrence Job
        /// </summary>
        /// <param name="executionDateTime">The execution date time. </param>
        /// <param name="isImmediate">True if Immediate else false</param>
        /// <returns>ICustomRecurrenceJob object.</returns>
        public ISingleRecurrenceJob ScheduleNewSingleRecurrenceJob(DateTime executionDateTime, bool isImmediate)
        {
            string uniqueName = this.EndUserFriendlyName + "[" + DateTime.Now.ToBinary() + "]";
            ISingleRecurrenceJob singleRecurrenceJob = new SingleRecurrenceJob(this.CallBackMethod, uniqueName, this._logger)
                                                            .WithTimeOut(this.TimeOut)
                                                            .WithOnStartCallBack(this.OnStartCallBack)
                                                            .WithOnEndCallBack(this.OnEndCallBack)
                                                            .WithOnExceptionCallBack(this.OnExceptionCallBack);

            singleRecurrenceJob.Schedule(executionDateTime, isImmediate);
            return singleRecurrenceJob;
        }

        /// <summary>
        /// Schedules the job at the specified execution date time.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        /// <param name="period">The period.</param>
        public void Schedule(DateTime executionDateTime, TimeSpan period)
        {
            try
            {
                this.DueTime = DueTimeFrom(executionDateTime);
                this.FirstExecution = DateTime.Now.Add(DueTime);
                this.Period = period;
                if (!IsScheduled)
                {
                    Timer = new Timer(TimerCallback, State, this.DueTime, this.Period);
                }
                else
                {
                    Timer.Change(this.DueTime, period);
                }
                this.IsScheduled = true;
                JobManager.TryAdd(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Schedules the job with the specified due time.
        /// </summary>
        /// <param name="dueTime">The due time.</param>
        /// <param name="period">The period.</param>
        public void Schedule(TimeSpan dueTime, TimeSpan period)
        {
            try
            {
                this.FirstExecution = DateTime.Now.Add(dueTime);
                if (!this.IsScheduled)
                {
                    Timer = new Timer(TimerCallback, State, dueTime, period);
                }
                else
                {
                    Timer.Change(dueTime, period);
                }
                this.IsScheduled = true;
                this.DueTime = dueTime;
                this.Period = period;
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
        /// <returns></returns>
        private TimeSpan DueTimeFrom(DateTime executionDateTime)
        {
            DateTime now = DateTime.Now;
            TimeSpan dueTime = new TimeSpan(0);

            if (executionDateTime > now)
                dueTime = executionDateTime - now;

            return dueTime;
        }
    }
}
