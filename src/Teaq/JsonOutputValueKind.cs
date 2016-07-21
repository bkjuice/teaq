using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Enumeration of possible JSON value kinds.
    /// </summary>
    public enum JsonOutputValueKind
    {
        /// <summary>
        /// The string value.
        /// </summary>
        StringValue,

        /// <summary>
        /// The number value.
        /// </summary>
        NumberValue,

        /// <summary>
        /// The boolean value.
        /// </summary>
        BooleanValue,

        /// <summary>
        /// The null value.s
        /// </summary>
        NullValue
    }
}
