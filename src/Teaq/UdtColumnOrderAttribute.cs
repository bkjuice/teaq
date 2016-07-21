using System;

namespace Teaq
{
    /// <summary>
    /// Attribute when put on a property to give it explicit ordering to match a user defined table type.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UdtColumnOrderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdtColumnOrderAttribute"/> class.
        /// </summary>
        public UdtColumnOrderAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtColumnOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public UdtColumnOrderAttribute(int order)
        {
            // NOTE: Do NOT change CTOR argument ordering here. Using CustomAttributeData.
            this.Order = order;
        }

        /// <summary>
        /// Gets the order for the column.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; private set; }
    }
}
