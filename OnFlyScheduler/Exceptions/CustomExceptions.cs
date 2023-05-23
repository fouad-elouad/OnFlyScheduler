using System;

namespace OnFlyScheduler.Exceptions
{
    /// <summary>
    /// JobAlreadyScheduledException Class.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class JobAlreadyScheduledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the<see cref="JobAlreadyScheduledException"/> class.
        /// </summary>
        public JobAlreadyScheduledException() : base() { }
        /// <summary>
        /// Initializes a new instance of the<see cref="JobAlreadyScheduledException"/> class.
        /// </summary>
        /// <param name="message">Message</param>
        public JobAlreadyScheduledException(string message) : base(message) { }
    }

}
