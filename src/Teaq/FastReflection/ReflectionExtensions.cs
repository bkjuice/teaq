using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Diagnostics.Contracts;

namespace Teaq.FastReflection
{
    /// <summary>
    /// Sponsor class for reflection extensions used by the fast reflection library.
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Converts the array of types to runtime type handles.
        /// </summary>
        /// <param name="types">The types to convert.</param>
        /// <returns>An array of <see cref="RuntimeTypeHandle"/>s.</returns>
        public static RuntimeTypeHandle[] ToRuntimeHandles(this Type[] types)
        {
            if (types == null)
            {
                return null;
            }

            return types.Transform(t => t.TypeHandle);
        }

        /// <summary>
        /// Converts the array of types to runtime type handles.
        /// </summary>
        /// <param name="parameters">The types to convert.</param>
        /// <returns>An array of <see cref="RuntimeTypeHandle"/>s.</returns>
        public static RuntimeTypeHandle[] ToRuntimeHandles(this ParameterInfo[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            return parameters.Transform(t => t.ParameterType.TypeHandle);
        }

        /// <summary>
        /// Converts the array of types to runtime type handles.
        /// </summary>
        /// <param name="instances">The types to convert.</param>
        /// <returns>An array of <see cref="RuntimeTypeHandle"/>s.</returns>
        public static RuntimeTypeHandle[] ToRuntimeHandles(this object[] instances)
        {
            if (instances == null)
            {
                return null;
            }

            return instances.Transform(t => Type.GetTypeHandle(t));
        }

        /// <summary>
        /// Converts the array of runtime handles to an array of types.
        /// </summary>
        /// <param name="types">The runtime handles to convert.</param>
        /// <returns>An array of <see cref="Type"/>s that corresponds to the array of provided type handles.</returns>
        public static Type[] FromRuntimeHandles(this RuntimeTypeHandle[] types)
        {
            if (types == null)
            {
                return null;
            }

            return types.Transform(t => Type.GetTypeFromHandle(t));
        }

        /// <summary>
        /// Converts reflected <see cref="FieldInfo" /> instances to a corresponding array of <see cref="FieldDescription" /> instances.
        /// </summary>
        /// <param name="infos">The field info instances.</param>
        /// <returns>
        /// A corresponding array of <see cref="FieldDescription" /> instances.
        /// </returns>
        public static FieldDescription[] ToFieldDescriptions(this FieldInfo[] infos)
        {
            if (infos == null)
            {
                return null;
            }

            return infos.Transform(i => new FieldDescription(i));
        }

        /// <summary>
        /// Creates a dictionary from the specified fields.
        /// </summary>
        /// <param name="items">The items that will populate the dictionary.</param>
        /// <returns>The dictionary keyed by name.</returns>
        public static Hashtable ToHashtable(this FieldDescription[] items)
        {
            return items.ToHashtable(i => i.MemberName, StringComparer.Ordinal);
        }

        /// <summary>
        /// Creates a dictionary from the specified methods.
        /// </summary>
        /// <param name="items">The items that will populate the dictionary.</param>
        /// <returns>The dictionary keyed by the method name and arguments.</returns>
        public static Hashtable ToHashtable(this MethodDescription[] items)
        {
            return items.ToHashtable(i => new MethodKey(i.MemberName, i.GetParameterTypeHandles()));
        }

        /// <summary>
        /// Converts reflected <see cref="PropertyInfo" /> instances to a corresponding array of <see cref="PropertyDescription" /> instances.
        /// </summary>
        /// <param name="infos">The <see cref="PropertyInfo" /> instances.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// A corresponding array of <see cref="PropertyDescription" /> instances.
        /// </returns>
        public static PropertyDescription[] ToPropertyDescriptions(this PropertyInfo[] infos, BindingFlags bindings)
        {
            if (infos == null)
            {
                return null;
            }

            return infos.Transform(i => new PropertyDescription(i, bindings));
        }

        /// <summary>
        /// Converts reflected <see cref="MethodInfo" /> instances to a corresponding array of <see cref="MethodDescription" /> instances.
        /// </summary>
        /// <param name="infos">The <see cref="MethodInfo" /> instances.</param>
        /// <returns>
        /// A corresponding array of <see cref="PropertyDescription" /> instances.
        /// </returns>
        public static MethodDescription[] ToMethodDescriptions(this MethodInfo[] infos)
        {
            if (infos == null)
            {
                return null;
            }

            return infos
                .Where(i => !(i?.IsGenericMethod).GetValueOrDefault(true))
                .Select(i => new MethodDescription(i))
                .ToArray();
        }

        /// <summary>
        /// Tries the get generic argument description.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns>
        /// The type description or null.
        /// </returns>
        /// <exception cref="System.ArgumentException">The provided argumentIndex value cannot be less than 0.</exception>
        public static Type TryGetGenericArgumentType(this Type targetType, int argumentIndex = 0)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(argumentIndex >= 0);

            var types = targetType.GetGenericArguments();
            if (argumentIndex < types?.GetLength(0))
            {
                return types[argumentIndex];
            }

            return null;
        }

        /// <summary>
        /// Creates a dictionary from the specified fields.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="items">The items that will populate the dictionary.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The hashtable.
        /// </returns>
        public static Hashtable ToHashtable<T, TKey>(this T[] items, Func<T, TKey> keySelector, IEqualityComparer comparer = null)
        {
            Contract.Requires(keySelector != null);

            if (items == null)
            {
                return null;
            }

            var table = comparer != null ?  new Hashtable(items.Length, comparer) : new Hashtable(items.Length);
            for (int i = 0; i < items.Length; ++i)
            {
                var item = items[i];
                if (item != null)
                {
                    var key = keySelector(item);
                    if (key != null)
                    {
                        table.Add(key, item);
                    }
                }
            }

            return table;
        }
    }
}
