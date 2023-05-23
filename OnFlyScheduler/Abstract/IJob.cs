
namespace OnFlyScheduler.Abstract
{
    /// <summary>
    /// The interface to be implemented by classes which represent a Job to be
    /// performed.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Releases all resources used by the current instance of OnFlyScheduler.IJob
        /// </summary>
        void Dispose();
    }
}
