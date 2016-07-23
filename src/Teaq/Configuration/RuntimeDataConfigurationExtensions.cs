using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace Teaq.Configuration
{
    /// <summary>
    /// Sponsor class to help with internalized handling of runtime configuration.
    /// </summary>
    internal static class RuntimeDataConfigurationExtensions
    {
        internal static StringBuilder AppendIdentifier(this StringBuilder builder, string identifier)
        {
            return builder.Append(identifier.EnsureBracketedIdentifier());
        }

        internal static string EnsureBracketedIdentifier(this string identifier)
        {
            Contract.Requires(string.IsNullOrEmpty(identifier) == false);

            // HOT path, if this is partially qualified, just use it...if the caller can't get this right, 
            // the error will make the issue obvious:
            if (identifier[0] == '[')
            {
                return identifier;
            }

            // per the reference source, this overload of concat is highly optimized to use underlying native code:
            return "[" + identifier + ']';
        }

        internal static Type EnsureConcreteType<T>(this IDataModel dataModel)
        {
            return dataModel?.GetConcreteType<T>() ?? typeof(T);
        }

        internal static IEntityConfiguration TryGetEntityConfig<T>(this IDataModel dataModel)
        {
            return dataModel?.GetEntityConfig(typeof(T));
        }
    }
}
