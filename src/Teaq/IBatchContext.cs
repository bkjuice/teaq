using System;

namespace Teaq
{
    /// <summary>
    /// Testable surrogate interface used to abstract data access control. 
    /// This interface reduces the data context surface that must be mocked for unit testing.
    /// </summary>
    public interface IBatchContext : IDisposable
    {
        /// <summary>
        /// Gets the target batch.
        /// </summary>
        QueryBatch QueryBatch { get; }
    }
}
