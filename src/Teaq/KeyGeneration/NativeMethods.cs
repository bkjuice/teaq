using System;
using System.Runtime.InteropServices;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Host class for native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Creates a new sequenced unique identifier using the native windows RPC library.
        /// </summary>
        /// <returns>The sequenced GUID.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the native call returns a non-zero result.</exception>
        public static Guid NewRpcSequencedGuid()
        {
            Guid guid;
            var hresult = UuidCreateSequential(out guid);
            if (hresult != 0)
            {
                throw new InvalidOperationException(
                    "Unable to invoke RPC library rpcrt4.dll :: UuidCreateSequential. Encountered non-zero return code: " + hresult);
            }

            return guid;
        }

        /// <summary>
        /// Invokes the Windows RPC library to get a sequential GUID.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>A windows error code or HRESULT.</returns>
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);
    }
}
