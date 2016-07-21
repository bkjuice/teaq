using System;

namespace Teaq
{
    /// <summary>
    /// Attribute when put on a property excludes it from automatic data table flattening.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UdtColumnExcludeAttribute : Attribute
    {
    }
}
