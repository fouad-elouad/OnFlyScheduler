# OnFlyScheduler
Is an efficient task scheduling class library designed for .NET applications. 

## Description
OnFlyScheduler allows you to schedule and execute tasks with ease. It offers a rich set of scheduling options, including one-time tasks, recurring tasks, and delayed tasks.

It provides a flexible and dynamic solution for managing and orchestrating tasks in real-time.

## Features
- [x] Target .NET 4.5.2
- [x] One-time tasks
- [x] Recurring tasks
- [x] Delayed tasks
- [x] Exception Handling
- [x] Logging Support
- [x] Unit Test Project
- [x] Extensibility

## Overview

### Pre-Task and Post-Task Execution
The library supports executing methods before and after the main task is performed. This feature enables you to set up initialization routines, perform necessary preparations,store jobs history or perform cleanup tasks before and after the main task execution.

### Extensibility
 The library is designed to be extensible, enabling integration with different logging libraries, dependency injection frameworks, and third-party components. This facilitates seamless integration into existing application architectures.

### Logging
The library supports integration with various logging frameworks, enabling detailed logging and monitoring of task execution.

### Exception Handling
In the event of an exception being thrown during task execution, the library provides a mechanism to handle and log these exceptions. You can configure a method to be called when an exception occurs, allowing you to handle and log errors gracefully and take appropriate actions. 

### Task Timeout
To prevent tasks from running indefinitely, the library allows you to specify a timeout duration for each task. If a task exceeds the defined timeout, it can be automatically terminated.

### JobManager for Real-Time Monitoring
The library includes a JobManager component that keeps track of all scheduled jobs and their current states in real time. This allows you to monitor the execution status of tasks and retrieve information about completed or ongoing tasks. The JobManager provides a centralized and convenient interface to manage and monitor all scheduled jobs within your application.

### Manual Scheduling from Recurrence Job
 In addition to the scheduled recurrence jobs, the library provides the ability to manually schedule a new single recurrence job from a recurrence job. If a recurrence job fails for any reason, you can initiate a new single recurrence job manually to execute the task immediately. This feature ensures that tasks can be executed promptly, even if there were issues with the regular recurrence schedule.

## Usage

 1- Clone or download the repository: To get started, clone or download the repository to your local machine.
 
 2- Open the solution file in Visual Studio 2019+: The solution file is located in the root directory of the project. Open this file in Visual Studio to start working with the project.
 
 3- Restore NuGet Packages
 
 4- Build and run the project
 
 5- Modify the project as needed for your own application: This template is designed to be a starting point for your application. Modify the class files, add new dependencies, or create new files as needed for your own application.
 

### Single Recurrence Job

 The library allows you to schedule a task that will be executed once at a specific time or after a specified delay. This feature is useful for tasks that need to be performed at a specific moment or after a certain interval.

 ```csharp

  public void Run_Task(TimeSpan timeOut)
{
    // Task to execute
}

ISingleRecurrenceJob singleRecurrenceJob = new SingleRecurrenceJob(Run_Task, nameof(Run_Task), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0)) // Task Tiemeout (1min)
                .WithOnStartCallBack(OnJobStart) // called before main task (optional)
                .WithOnEndCallBack(OnJobEnd) // called after main task (optional)
                .WithOnExceptionCallBack(OnException); // called when an exception has been thrown (optional)

            singleRecurrenceJob.Schedule(TimeSpan.FromHours(2), false); // 2 hours delay
 ```

 ```csharp
            singleRecurrenceJob.Schedule(new DateTime(2023,01,01,00,00,00), false); // specific time
 ```
 
 ```csharp
            singleRecurrenceJob.Schedule(new DateTime(), true); // Executes the job immediately
 ```
 
### Daily Recurrence Job

With the daily recurrence job feature, you can schedule tasks to be executed daily at a specific time. This is ideal for tasks that need to be performed on a daily basis, such as generating reports, performing maintenance activities or webservices call.


```csharp

 CallBackMethod Run_Task = (TimeSpan timeOut) => { }; // Task to execute

IDailyRecurrenceJob dailyRecurrenceJob = new DailyRecurrenceJob(Run_Task, nameof(Run_Task), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnStartCallBack(jobState.OnJobStart)
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            dailyRecurrenceJob.Schedule(new DateTime(2023,01,01,12,30,00)); // specific day time
```

### Custom Recurrence Job

The library provides flexibility for defining custom recurrence patterns based on your specific needs. You can configure the library to execute tasks at specific intervals or according to any other custom recurrence pattern required by your application.

```csharp

ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task, nameof(Run_Task), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            var customPeriod = TimeSpan.FromDays(7); // weekly period
            customRecurrenceJob.Schedule(TimeSpan.FromSeconds(1), customPeriod);
```

### Manual Scheduling from Recurrence Job

```csharp

ICustomRecurrenceJob customRecurrenceJob = new CustomRecurrenceJob(Run_Task, nameof(Run_Task), new ConsoleLogger())
                .WithTimeOut(new TimeSpan(0, 1, 0))
                .WithOnEndCallBack(jobState.OnJobEnd)
                .WithOnExceptionCallBack(jobState.OnException);

            var customPeriod = TimeSpan.FromHours(12); // 12h period
            customRecurrenceJob.Schedule(TimeSpan.FromHours(1), customPeriod);

            ISingleRecurrenceJob singleRecurrenceJob = customRecurrenceJob
            .ScheduleNewSingleRecurrenceJob(new DateTime(), true); // Executes the job immediately
            
```

## Support
If you are having problems, please let us know by [raising a new issue](https://github.com/fouad-elouad/OnFlyScheduler/issues/new/choose).

## License
This project is licensed with the [MIT license](LICENSE).
