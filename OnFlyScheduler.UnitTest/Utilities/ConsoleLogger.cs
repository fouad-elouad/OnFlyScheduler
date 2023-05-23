using OnFlyScheduler.Abstract;
using System;
using System.Diagnostics;

namespace OnFlyScheduler.UnitTest.Utilities
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        public void Debug(string msg)
        {
            Trace.WriteLine(nameof(Debug)+ ": " + msg);
        }
        public void Info(string msg)
        {
            Trace.WriteLine(nameof(Info) + ": " + msg);
        }
        public void Warn(string msg)
        {
            Trace.WriteLine(nameof(Warn) + ": " + msg);
        }
        public void Error(string msg)
        {
            Trace.WriteLine(nameof(Error) + ": " + msg);
        }
        public void Fatal(string msg)
        {
            Trace.WriteLine(nameof(Fatal) + ": " + msg);
        }
    }
}
