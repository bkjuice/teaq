using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Testable surrogate interface used to abstract data access control. 
    /// This interface reduces the data context surface that must be mocked for unit testing.
    /// </summary>
    public interface IBatchWriter: IBatchContext
    {
        /// <summary>
        /// Saves all pending changes to the backing store.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves all changes asynchronously and returns a single awaitable handle.
        /// </summary>
        /// <returns>The awaitable task.</returns>
        Task SaveChangesAsync();
    }
}
