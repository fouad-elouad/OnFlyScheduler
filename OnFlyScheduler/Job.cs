using OnFlyScheduler.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnFlyScheduler
{

    /// <summary>
    /// Job class
    /// Represents an abstract class for scheduled operations.
    /// </summary>
    /// <seealso cref="OnFlyScheduler.Abstract.IJob" />
    public abstract class Job : IJob
    {
        /// <summary>
        /// Provides a mechanism for executing a method at specified intervals
        /// </summary>
        protected Timer Timer;

        /// <summary>
        /// A System.Threading.TimerCallback delegate representing a method to be executed.
        /// </summary>
        protected TimerCallback TimerCallback;

        /// <summary>
        /// Gets or sets the job Friendly Name.
        /// </summary>
        /// <value>
        /// job Name.
        /// </value>
        public string EndUserFriendlyName { get; protected set; }

        /// <summary>
        /// The System.TimeSpan representing the amount of time to delay before the callback
        /// parameter invokes its methods. Specify negative one (-1) milliseconds to prevent
        /// the timer from starting. Specify zero (0) to start the timer immediately.
        /// </summary>
        /// <value>
        /// The due time.
        /// </value>
        public TimeSpan DueTime { get; protected set; }

        /// <summary>
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </summary>
        /// <value>
        /// The period interval.
        /// </value>
        public TimeSpan Period { get; protected set; }
        public bool IsScheduled { get; protected set; }

        /// <summary>
        /// Gets or sets the Job state.
        /// </summary>
        /// <value>
        /// The job state.
        /// </value>
        public State State { get; protected set; }

        /// <summary>
        /// CallBack Method to execute
        /// </summary>
        /// <value>
        /// CallBack Method
        /// </value>
        public CallBackMethod CallBackMethod { get; protected set; }

        private DateTime _FirstExecution;

        /// <summary>
        /// Gets The first execution Date time or sets the first execution Date time and The last scheduled run date.
        /// </summary>
        /// <value>
        /// The first execution Date time.
        /// </value>
        public DateTime FirstExecution
        {
            get { return _FirstExecution; }
            protected set { _FirstExecution = value; LastScheduledRunDate = value; }
        }

        /// <summary>
        /// Gets or sets the last scheduled run date.
        /// every time first execution Date is changed last scheduled run date is also changed
        /// used internally
        /// </summary>
        /// <value>
        /// The last scheduled run date.
        /// </value>
        public DateTime? LastScheduledRunDate { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [Job is scheduled for only one execution].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [single recurrence]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSingleRecurrence { get; protected set; }

        /// <summary>
        /// Gets or sets the on exception call back.
        /// this method is executed if any exception if thrown
        /// </summary>
        /// <value>
        /// The on exception call back.
        /// </value>
        public OnExceptionCallBack OnExceptionCallBack { get; protected set; }

        /// <summary>
        /// Gets or sets the Canceleation time out.
        /// </summary>
        /// <value>
        /// The time out.
        /// </value>
        public TimeSpan TimeOut { get; protected set; }

        public OnStartCallBack OnStartCallBack { get; protected set; }
        public OnEndCallBack OnEndCallBack { get; protected set; }

        protected ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job" /> class.
        /// the default timeout is 1h
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">the Job friendly name .</param>
        protected Job(CallBackMethod callBackMethod, string endUserFriendlyName)
        {
            CallBackMethod = callBackMethod;
            TimerCallback = TimerCallbackMethod;
            State = new State();
            IsScheduled = false;
            EndUserFriendlyName = endUserFriendlyName;
            TimeOut = TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Job" /> class.
        /// the default timeout is 1h
        /// </summary>
        /// <param name="callBackMethod">The call back method.</param>
        /// <param name="endUserFriendlyName">the Job friendly name .</param>
        /// <param name="logger">the logger.</param>
        protected Job(CallBackMethod callBackMethod, string endUserFriendlyName, ILogger logger)
        {
            CallBackMethod = callBackMethod;
            TimerCallback = TimerCallbackMethod;
            State = new State();
            IsScheduled = false;
            EndUserFriendlyName = endUserFriendlyName;
            TimeOut = TimeSpan.FromHours(1);
            _logger = logger;
        }

        protected IJob WithTimeOut(TimeSpan timeOut)
        {
            TimeOut = timeOut;
            return this;
        }

        public IJob WithOnExceptionCallBack(OnExceptionCallBack onExceptionCallBack)
        {
            OnExceptionCallBack = onExceptionCallBack;
            return this;
        }

        public IJob WithOnStartCallBack(OnStartCallBack onStartCallBack)
        {
            OnStartCallBack = onStartCallBack;
            return this;
        }

        public IJob WithOnEndCallBack(OnEndCallBack onEndCallBack)
        {
            OnEndCallBack = onEndCallBack;
            return this;
        }

        /// <summary>
        /// Releases all resources used by the current instance of OnFlyScheduler.Job
        /// </summary>
        public void Dispose()
        {
            _logger?.Warn($"Job: {this.EndUserFriendlyName} Disposed.");
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            JobManager.Remove(this);
            TimerCallback = null;
            Timer.Dispose();
        }

        /// <summary>
        /// synchronously Execute the asynchronous CallBackMethod
        /// </summary>
        /// <param name="state">The job state object.</param>
        private void TimerCallbackMethod(object state)
        {
            if (Monitor.TryEnter(state))
            {
                Exception raisedException = null;
                try
                {
                    StartCallback(state);
                    using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                    {
                        ExecuteCallbackAsync(cancellationTokenSource).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }

                catch (TaskCanceledException ex)
                {
                    _logger?.Error($"Job: {this.EndUserFriendlyName}, TaskCanceledException with time out {TimeOut.TotalSeconds} seconds" );
                    State State = (State)state;
                    State.IsCancelled = true;
                    raisedException = ex;
                    OnExceptionCallBack?.Invoke(ex);
                }

                catch (Exception ex)
                {
                    _logger?.Error($"Job: {this.EndUserFriendlyName}, {ex} Exception caught." );
                    raisedException = ex;
                    OnExceptionCallBack?.Invoke(ex);
                }

                finally
                {
                    EndCallback(state, raisedException);
                    Monitor.Exit(state);
                }
            }
        }

        /// <summary>
        /// Resets the timer to optimize period precision.
        /// </summary>
        private void ResetTimer()
        {
            try
            {
                this.LastScheduledRunDate = LastScheduledRunDate?.Add(Period);
                TimeSpan? dueTime = this.LastScheduledRunDate - DateTime.Now;
                if (dueTime != null && dueTime > TimeSpan.Zero)
                    Timer.Change(dueTime.Value.Add(TimeSpan.FromMilliseconds(100)), Period);
                else
                    this.LastScheduledRunDate = State.LastRunDate?.Add(Period);
            }
            catch (Exception ex)
            {
                // Exception not thrown, this method optimize period precision and must not exit job execution anyway
                _logger?.Error($"ResetTimer: {this.EndUserFriendlyName}, {ex} Exception caught.");
            }
        }

        /// <summary>
        /// Executes the main callback method.
        /// if the excecution is timed out Throw TaskCanceledException
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">The operation has Cancelled due to timeOut.</exception>
        private async Task ExecuteCallbackAsync(CancellationTokenSource cancellationTokenSource)
        {
            Task callBackTask = Task.Run(() => CallBackMethod.Invoke(TimeOut), cancellationTokenSource.Token);
            cancellationTokenSource.CancelAfter(TimeOut);
            Task firstCompletedTask = await Task.WhenAny(callBackTask, Task.Delay(TimeOut, cancellationTokenSource.Token)).ConfigureAwait(false);
            if (firstCompletedTask == callBackTask)
            {
                await callBackTask.ConfigureAwait(false);
            }
            else
            {
                cancellationTokenSource.Cancel();
                throw new TaskCanceledException("The operation has Cancelled due to timeOut.");
            }
        }

        /// <summary>
        /// Used in callback Method start.
        /// </summary>
        /// <param name="StateObj">The state object.</param>
        private void StartCallback(object StateObj)
        {
            try
            {
                State State = (State)StateObj;
                State.LastRunDate = DateTime.Now;
                State.IsRunning = true;
                State.IsCancelled = false;
                OnStartCallBack?.Invoke(this);
                _logger?.Info($"Job: {this.EndUserFriendlyName} StartCallback.");
            }
            catch (InvalidCastException ex)
            {
                _logger?.Error($"Job: {this.EndUserFriendlyName}, StartCallback {ex} Exception caught.");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Job: {this.EndUserFriendlyName}, StartCallback {ex} Exception caught.");
                throw;
            }
        }

        /// <summary>
        /// Used in callback Method end.
        /// </summary>
        /// <param name="StateObj">The state object.</param>
        /// <param name="raisedException">The raised exception.</param>
        private void EndCallback(object StateObj, Exception raisedException)
        {
            try
            {

                if (raisedException == null)
                    _logger?.Info($"Job: {this.EndUserFriendlyName} EndCallback successfully.");
                else
                    _logger?.Info($"Job: {this.EndUserFriendlyName} EndCallback with Exceptions.");

                State State = (State)StateObj;
                State.LastExceptionOccured = raisedException;
                if (State.IsRunning)
                {
                    State.LastEndDate = DateTime.Now;
                    State.IsRunning = false;
                }

                if (!this.IsSingleRecurrence)
                {
                    ResetTimer();
                    OnEndCallBack?.Invoke(this);
                }
                else
                {
                    OnEndCallBack?.Invoke(this);
                    // SingleRecurrence => Release timer
                    Dispose();
                }
            }
            catch (InvalidCastException ex)
            {
                // Custom Exception
                _logger?.Error($"Job: {this.EndUserFriendlyName}, EndCallback {ex} Exception caught.");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Job: {this.EndUserFriendlyName}, EndCallback {ex} Exception caught.");
                throw;
            }
        }
    }
}
