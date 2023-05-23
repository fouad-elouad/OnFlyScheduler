using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnFlyScheduler.Abstract;
using OnFlyScheduler.SpecificJobs;
using OnFlyScheduler.UnitTest.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnFlyScheduler.UnitTest
{
    [TestClass]
    public class UnitTest
    {

        [TestMethod]
        public void TestSucceededJob()
        {
            JobState jobState = new JobState();
            int counter = 0;
            CallBackMethod Run_Task_SucceededJob = (TimeSpan timeOut) =>
            {
                ThreadSleepSeconds(10); counter++;
            };

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_SucceededJob, nameof(Run_Task_SucceededJob), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnStartCallBack(jobState.OnJobStart)
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(10));
            Job runningJob = JobManager.GetByFriendlyName(nameof(Run_Task_SucceededJob));

            Assert.IsTrue(runningJob.IsScheduled);
            Assert.IsFalse(runningJob.State.IsRunning);

            ThreadSleepSeconds(2); // wait Task to start

            Assert.AreEqual(0, counter);
            Assert.IsTrue(runningJob.State.IsRunning);

            ThreadSleepSeconds(15); // wait Task to finish

            Assert.IsFalse(runningJob.State.IsRunning);
            Assert.AreEqual(1, counter);
            Assert.IsTrue(jobState.OnJobEndExecuted);
            Assert.IsFalse(jobState.OnExceptionExecuted);

            runningJob.Dispose();
        }

        [TestMethod]
        public void TestJobPeriods()
        {
            JobState jobState = new JobState();
            int counter = 0;
            CallBackMethod Run_Task_SucceededJob = (TimeSpan timeOut) => { counter++; };

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_SucceededJob, nameof(Run_Task_SucceededJob), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            var period = TimeSpan.FromSeconds(5);
            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(1), period);

            Assert.AreEqual(0, counter);

            ThreadSleepSeconds(6); // wait Task to finish
            Assert.AreEqual(1, counter);

            ThreadSleepSeconds(5); // wait Task to finish
            Assert.AreEqual(2, counter);

            ThreadSleepSeconds(5); // wait Task to finish
            Assert.AreEqual(3, counter);

            customRecurrenceJob.Dispose();
        }

        [TestMethod]
        public void TestStopJob()
        {
            JobState jobState = new JobState();
            int counter = 0;
            CallBackMethod Run_Task_SucceededJob = (TimeSpan timeOut) => { counter++; };

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_SucceededJob, nameof(Run_Task_SucceededJob), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            var period = TimeSpan.FromSeconds(5);
            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(1), period);

            Assert.AreEqual(0, counter);

            ThreadSleepSeconds(6); // wait Task to finish
            Assert.AreEqual(1, counter);

            customRecurrenceJob.Dispose();

            ThreadSleepSeconds(5);
            Assert.AreEqual(1, counter); // job already stopped
        }

        [TestMethod]
        public void TestSingleRecurrenceJob()
        {
            JobState jobState = new JobState();
            int counter = 0;
            CallBackMethod Run_Task_SucceededJob = (TimeSpan timeOut) => { counter++; };

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_SucceededJob, nameof(Run_Task_SucceededJob), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            var period = TimeSpan.FromSeconds(5);
            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(1), period);

            Assert.AreEqual(0, counter);

            customRecurrenceJob.Dispose();

            ISingleRecurrenceJob singleRecurrenceJob = customRecurrenceJob.ScheduleNewSingleRecurrenceJob(DateTime.Now.AddSeconds(10), false);
            ThreadSleepSeconds(6);
            Assert.AreEqual(0, counter);

            ThreadSleepSeconds(5);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void TestTimeout()
        {
            JobState jobState = new JobState();

            CallBackMethod Run_Task_8s = (TimeSpan timeOut) => ThreadSleepSeconds(8);

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_8s, nameof(Run_Task_8s), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 0, 4)) // time out in 4 seconds
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(10));

            Assert.IsFalse(jobState.OnJobEndExecuted);
            Assert.IsFalse(jobState.OnExceptionExecuted);

            ThreadSleepSeconds(10);

            Assert.IsTrue(jobState.OnJobEndExecuted);
            Assert.IsTrue(jobState.OnExceptionExecuted);
            Assert.IsInstanceOfType(jobState.Exception, typeof(TaskCanceledException));

            customRecurrenceJob.Dispose();
        }

        [TestMethod]
        public void TestCustomException()
        {
            JobState jobState = new JobState();

            CallBackMethod Run_Task_Exception = (TimeSpan timeOut) => throw new NullReferenceException();

            ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task_Exception, nameof(Run_Task_Exception), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 0, 10))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(10));

            Assert.IsFalse(jobState.OnJobEndExecuted);
            Assert.IsFalse(jobState.OnExceptionExecuted);

            ThreadSleepSeconds(10);

            Assert.IsTrue(jobState.OnJobEndExecuted);
            Assert.IsTrue(jobState.OnExceptionExecuted);
            Assert.IsInstanceOfType(jobState.Exception, typeof(NullReferenceException));

            customRecurrenceJob.Dispose();
        }

        public void ThreadSleepSeconds(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }
    }
}
