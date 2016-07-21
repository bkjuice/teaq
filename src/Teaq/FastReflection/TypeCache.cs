using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Teaq.FastReflection
{
    /// <summary>
    /// Static cache of reflected types.
    /// </summary>
    public static class TypeCache
    {
        /// <summary>
        /// The cached types. 
        /// </summary>
        private static readonly Hashtable types = new Hashtable(4096);

        /// <summary>
        /// The synclock used to ensure thread safety.
        /// </summary>
        private static readonly object synclock = new object();

        /// <summary>
        /// The object description.
        /// </summary>
        private static readonly TypeDescription objectDescription = typeof(object).GetTypeDescription();

        /// <summary>
        /// Gets the description for <see cref="System.Object"/>.
        /// </summary>
        public static TypeDescription ObjectDescription
        {
            get
            {
                Contract.Ensures(Contract.Result<TypeDescription>() != null);

                return objectDescription;
            }
        }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        /// <param name="type">The runtime type identifier.</param>
        /// <returns>The type description.</returns>
        public static TypeDescription GetTypeDescription(this Type type)
        {
            Contract.Requires(type != null);

            return GetTypeDescription(type, null);
        }

        /// <summary>
        /// Gets the type description with IL accessors eagerly generated.
        /// </summary>
        /// <param name="type">The target.</param>
        /// <param name="includedMembers">The included members.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The type description instance.
        /// </returns>
        public static TypeDescription GetTypeDescription(
            this Type type,
            MemberTypes includedMembers,
            BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            Contract.Requires(type != null);

            return GetTypeDescription(type, (t) => Initialize(t, includedMembers, bindings));
        }

        /// <summary>
        /// Clears all cached types, fields, properties and methods.
        /// </summary>
        public static void Clear()
        {
            types.Clear();
        }

        /// <summary>
        /// Evaluates the type of the member.
        /// </summary>
        /// <param name="type">The type of member.</param>
        /// <returns>The type description instance.</returns>
        private static TypeDescription CreateDescription(Type type)
        {
            Contract.Requires(type != null);

            TypeDescription result = null;
            if (type.TypeHandle.IsPrimitiveOrStringOrNullable())
            {
                result = CreatePrimitiveType(type);
            }
            else if (type.IsArray)
            {
                result = CreateArrayType(type);
            }
            else if (typeof(MulticastDelegate).IsAssignableFrom(type))
            {
                result = new TypeDescription(type);
                result.CommonUse = CommonUseType.DelegateType;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                result = CreateDictionaryType(type);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                result = CreateCollectionType(type, CommonUseType.ListType, true, true);
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                result = CreateCollectionType(type, CommonUseType.EnumerableType, true, true);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                result = CreateCollectionType(type, CommonUseType.EnumerableType, false, true);
            }
            else if (type.IsEnum)
            {
                result = new TypeDescription(type);
                result.IsPrimitiveOrString = true;
                result.IsPrimitiveOrStringOrNullable = true;
                result.IsEnum = true;
                result.CommonUse = CommonUseType.EnumType;
            }
            else if (type.IsGenericType)
            {
                result = new TypeDescription(type);
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum)
                {
                    result.IsPrimitiveNullable = true;
                    result.IsPrimitiveOrStringOrNullable = true;
                    result.IsNullableEnum = true;
                    result.CommonUse = CommonUseType.NullableEnumType;
                }

                result.IsAbstract = type.IsAbstract;
                result.IsGeneric = true;
            }

            if (result == null)
            {
                result = new TypeDescription(type);
                result.IsAbstract = type.IsAbstract;
                result.IsGeneric = type.IsGenericType;
            }
                        
            return result;
        }

        /// <summary>
        /// Creates the array type description.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The array type description.</returns>
        private static TypeDescription CreateArrayType(Type type)
        {
            Contract.Requires(type != null);

            var arrayType = new ArrayDescription(type);
            arrayType.IsArray = true;
            arrayType.CommonUse = CommonUseType.ArrayType;
            arrayType.ElementType = type.GetElementType().GetTypeDescription();
            arrayType.InitializeAccessors();
            arrayType.IsAbstract = type.IsAbstract;
            arrayType.IsGeneric = type.IsGenericType;

            return arrayType;
        }

        /// <summary>
        /// Creates the array type description.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The array type description.</returns>
        private static TypeDescription CreateDictionaryType(Type type)
        {
            Contract.Requires(type != null);

            var result = new DictionaryDescription(type);
            result.IsIDictionary = true;
            result.IsICollection = true;
            result.IsIEnumerable = true;
            result.CommonUse = CommonUseType.DictionaryType;
            result.IsAbstract = type.IsAbstract;
            result.IsGeneric = type.IsGenericType;
            result.KeyType = result.TryGetGenericArgumentDescription() ?? objectDescription;
            result.ValueType = result.TryGetGenericArgumentDescription(1) ?? objectDescription;
            return result;
        }

        /// <summary>
        /// Creates the collection type description.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="usage">The usage.</param>
        /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
        /// <param name="isEnumerable">if set to <c>true</c> [is enumerable].</param>
        /// <returns>The collection description.</returns>
        private static TypeDescription CreateCollectionType(Type type, CommonUseType usage, bool isCollection, bool isEnumerable)
        {
            Contract.Requires(type != null);

            var result = new CollectionDescription(type);
            result.IsICollection = isCollection;
            result.IsIEnumerable = isEnumerable;
            result.CommonUse = usage;
            result.IsAbstract = type.IsAbstract;
            result.IsGeneric = type.IsGenericType;
            result.ItemType = result.TryGetGenericArgumentDescription() ?? objectDescription;
            result.ResolveAddMethod();
            return result;
        }

        /// <summary>
        /// Evaluates the primitive type. Extracted for readability.
        /// </summary>
        /// <param name="typeOfMember">The type of member.</param>
        /// <returns>The type description instance.</returns>
        private static TypeDescription CreatePrimitiveType(Type typeOfMember)
        {
            Contract.Requires(typeOfMember != null);

            var result = new TypeDescription(typeOfMember);
            result.IsPrimitiveOrString = typeOfMember.IsPrimitiveOrString();
            result.IsPrimitiveNullable = typeOfMember.IsPrimitiveNullable();
            result.IsPrimitiveOrStringOrNullable = true;

            if (result.IsPrimitiveNullable)
            {
                result.CommonUse = CommonUseType.NullablePrimitiveType;
            }
            else if (typeof(string) == typeOfMember)
            {
                result.CommonUse = CommonUseType.StringType;
            }
            else
            {
                result.CommonUse = CommonUseType.PrimitiveType;
            }

            return result;
        }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        /// <param name="type">The runtime type identifier.</param>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>
        /// The type description.
        /// </returns>
        private static TypeDescription GetTypeDescription(this Type type, Action<TypeDescription> initAction)
        {
            Contract.Requires(type != null);

            var t = types[type] as TypeDescription;
            if (t != null)
            {
                return t;
            }

            lock (synclock)
            {
                t = types[type] as TypeDescription;
                if (t != null)
                {
                    return t;
                }

                t = CreateDescription(type);
                initAction?.Invoke(t);
                types.Add(type, t);
                return t;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescription" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="members">The included members.</param>
        /// <param name="bindings">The bindings.</param>
        private static void Initialize(TypeDescription type, MemberTypes members, BindingFlags bindings)
        {
            Contract.Requires(type != null);

            if (type.HasDefaultCtor)
            {
                type.InitializeDefaultCtor();
            }

            if (members.HasFlag(MemberTypes.Property))
            {
                type.ReflectProperties(bindings);
            }

            if (members.HasFlag(MemberTypes.Field))
            {
                type.ReflectFields(bindings);
            }

            if (members.HasFlag(MemberTypes.Method))
            {
                type.ReflectMethods(bindings);
            }
        }
    }
}
