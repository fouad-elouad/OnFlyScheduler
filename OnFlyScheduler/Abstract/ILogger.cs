
namespace OnFlyScheduler.Abstract
{
    /// <summary>
    /// Implement this interface for internal logging
    /// </summary>
    public interface ILogger
    {
        void Debug(string msg);
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
        void Fatal(string msg);
    }
}
