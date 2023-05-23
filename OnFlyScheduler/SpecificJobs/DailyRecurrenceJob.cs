using OnFlyScheduler.Abstract;
using System;
using System.Threading;

namespace OnFlyScheduler.SpecificJobs
{
    /// <summary>
    /// DailyRecurrenceJob interface
    /// used for daily execution job mechanism.
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Abstract.IJob" />
    public interface IDailyRecurrenceJob : IJob
    {
        /// <summary>
        /// Schedules the job at the specified execution date time.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        void Schedule(DateTime executionDateTime);

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        IDailyRecurrenceJob WithTimeOut(TimeSpan timeOut);

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">Exception CallBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        IDailyRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack);

        /// <summary>
        /// Set  StartCallBack.
        /// </summary>
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        IDailyRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack);

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        IDailyRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack);

        /// <summary>
        /// Set Schedule New Single Recurrence Job
        /// </summary>
        /// <param name="executionDateTime">The execution date time. </param>
        /// <param name="isImmediate">True if Immediate else false</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
        ISingleRecurrenceJob ScheduleNewSingleRecurrenceJob(DateTime executionDateTime, bool isImmediate);
    }

    /// <summary>
    /// DailyRecurrenceJob class
    /// used for daily execution job mechanism.
    /// this class can not be inherited
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Job" />
    /// <seealso cref="OnFlyScheduler.IDailyRecurrenceJob" />
    public sealed class DailyRecurrenceJob : Job, IDailyRecurrenceJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        public DailyRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName)
            : base(callBackMethod, endUserFriendlyName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRecurrenceJob" /> class.
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">End name of the user friendly.</param>
        /// <param name="logger">the logger.</param>
        public DailyRecurrenceJob(CallBackMethod callBackMethod, string endUserFriendlyName, ILogger logger)
            : base(callBackMethod, endUserFriendlyName, logger)
        {
        }

        /// <summary>
        /// Set time out
        /// </summary>
        /// <param name="timeOut">The time out</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        public new IDailyRecurrenceJob WithTimeOut(TimeSpan timeOut)
        {
            return (IDailyRecurrenceJob) base.WithTimeOut(timeOut);
        }

        /// <summary>
        /// Set Exception CallBack.
        /// </summary>
        /// <param name="onExceptionCallBack">Exception CallBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        public new IDailyRecurrenceJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack)
        {
            return (IDailyRecurrenceJob) base.WithOnExceptionCallBack(onExceptionCallBack);
        }

        /// <summary>
        /// Set  StartCallBack.
        /// </summary>
        /// <param name="onStartCallBack">Start CallBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        public new IDailyRecurrenceJob WithOnStartCallBack(OnStartCallBack onStartCallBack)
        {
            return (IDailyRecurrenceJob) base.WithOnStartCallBack(onStartCallBack);
        }

        /// <summary>
        /// Set EndCallBack.
        /// </summary>
        /// <param name="onEndCallBack">End callBack.</param>
        /// <returns>IDailyRecurrenceJob object.</returns>
        public new IDailyRecurrenceJob WithOnEndCallBack(OnEndCallBack onEndCallBack)
        {
            return (IDailyRecurrenceJob) base.WithOnEndCallBack(onEndCallBack);
        }

        /// <summary>
        /// Set Schedule New Single Recurrence Job
        /// </summary>
        /// <param name="executionDateTime">The execution date time. </param>
        /// <param name="isImmediate">True if Immediate else false</param>
        /// <returns>ISingleRecurrenceJob object.</returns>
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
        public void Schedule(DateTime executionDateTime)
        {
            try
            {
                this.DueTime = DueTimeFrom(executionDateTime);
                this.FirstExecution = DateTime.Now.Add(DueTime);
                this.Period = TimeSpan.FromDays(1);

                if (!this.IsScheduled)
                {
                    this.Timer = new Timer(this.TimerCallback, this.State, this.DueTime, this.Period);
                }
                else
                {
                    this.Timer.Change(DueTime, Period);
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
        /// Gets timer DueTime from executionDateTime.
        /// </summary>
        /// <param name="executionDateTime">The execution date time.</param>
        /// <returns>DueTime</returns>
        private TimeSpan DueTimeFrom(DateTime executionDateTime)
        {
            DateTime now = DateTime.Now;
            TimeSpan dueTime;
            DateTime dateTime;
            if (executionDateTime > now)
                dueTime = executionDateTime - now;
            else
            {
                 dateTime = now.Date.AddHours(executionDateTime.Hour)
                                       .AddMinutes(executionDateTime.Minute)
                                       .AddSeconds(executionDateTime.Second);
                if (dateTime > now)
                    dueTime = dateTime - now;
                else
                    dueTime = dateTime.AddDays(1) - now;
            }
            return dueTime;
        }
    }
}
