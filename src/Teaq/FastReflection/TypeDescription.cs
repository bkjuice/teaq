using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Serialization;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght type description.
    /// </summary>
    public class TypeDescription : MemberDescription, IEquatable<Type>
    {
        /// <summary>
        /// The default empty "no modifiers" array singleton.
        /// </summary>
        private static readonly ParameterModifier[] defaultNoModifiers = new ParameterModifier[] { };

        /// <summary>
        /// The empty property array.
        /// </summary>
        private static readonly PropertyDescription[] emptyProps = new PropertyDescription[0];

        /// <summary>
        /// The empty fields array.
        /// </summary>
        private static readonly FieldDescription[] emptyFields = new FieldDescription[0];

        /// <summary>
        /// The empty methods array.
        /// </summary>
        private static readonly MethodDescription[] emptyMethods = new MethodDescription[0];

        /// <summary>
        /// The default ctor and any additional ctors if added by the API consumer.
        /// </summary>
        private readonly Hashtable ctors = new Hashtable();

        /// <summary>
        /// The cached field accessors keyed by binding flags.
        /// </summary>
        private readonly Hashtable fieldsCache = new Hashtable();

        /// <summary>
        /// The cached property accessors keyed by binding flags.
        /// </summary>
        private readonly Hashtable propertiesCache = new Hashtable();

        /// <summary>
        /// The cached property accessors keyed by binding flags.
        /// </summary>
        private readonly Hashtable indexedPropertiesCache = new Hashtable();

        /// <summary>
        /// The cached method accessors keyed by binding flags.
        /// </summary>
        private readonly Hashtable methodsCache = new Hashtable();

        /// <summary>
        /// The default ctor information lazily reflected on first use.
        /// </summary>
        private readonly Lazy<ConstructorInfo> defaultCtorInfo;

        /// <summary>
        /// The properties associated with the type indexed by name.
        /// </summary>
        private Hashtable propertiesByName;

        /// <summary>
        /// The fields associated with the type indexed by name.
        /// </summary>
        private Hashtable fieldsByName;

        /// <summary>
        /// The methods associated with the type indexed by name.
        /// </summary>
        private Hashtable methodsByName;

        /// <summary>
        /// The default ctor invocation.
        /// </summary>
        private Func<object[], object> defaultCtor;

        /// <summary>
        /// The current field bindings.
        /// </summary>
        private BindingFlags currentFieldBindings;

        /// <summary>
        /// The current property bindings.
        /// </summary>
        private BindingFlags currentPropertyBindings;

        /// <summary>
        /// The current method bindings.
        /// </summary>
        private BindingFlags currentMethodBindings;

        /// <summary>
        /// The current fields.
        /// </summary>
        private FieldDescription[] currentFields = emptyFields;

        /// <summary>
        /// The current properties.
        /// </summary>
        private PropertyDescription[] currentProperties = emptyProps;

        /// <summary>
        /// The current indexed properties.
        /// </summary>
        private PropertyDescription[] currentIndexedProperties = emptyProps;

        /// <summary>
        /// The current methods.
        /// </summary>
        private MethodDescription[] currentMethods = emptyMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescription" /> class without generating accessor IL.
        /// </summary>
        /// <param name="type">The type.</param>
        internal TypeDescription(Type type)
            : base(type, type)
        {
            Contract.Requires(type != null);

            this.TypeHandle = type.TypeHandle;
            this.IsSealed = type.IsSealed;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;

            this.defaultCtorInfo = new Lazy<ConstructorInfo>(() =>
                this.ReflectedType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic,
                Type.DefaultBinder,
                Type.EmptyTypes,
                defaultNoModifiers));

            this.MemberType = this;
        }

        /// <summary>
        /// Gets the unique name for the type.
        /// </summary>
        public string AssemblyQualifiedName { get; private set; }

        /// <summary>
        /// Gets the <see cref="RuntimeTypeHandle"/> for the target type.
        /// </summary>
        public RuntimeTypeHandle TypeHandle { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the type has a default constructor.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance has a default constructor; false otherwise, <c>false</c>.
        /// </value>
        public bool HasDefaultCtor
        {
            get
            {
                // This property is the reason why System.Lazy is used:
                return this.defaultCtorInfo.Value != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type description [has reflected properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if type description [has reflected properties]; otherwise, <c>false</c>.
        /// </value>
        public bool HasReflectedProperties
        {
            get
            {
                return !ReferenceEquals(this.currentProperties, emptyProps);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type description [has reflected indexed properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if type description [has reflected indexed properties]; otherwise, <c>false</c>.
        /// </value>
        public bool HasReflectedIndexedProperties
        {
            get
            {
                return this.currentIndexedProperties != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether type description [has reflected fields].
        /// </summary>
        /// <value>
        ///   <c>true</c> if type description [has reflected fields]; otherwise, <c>false</c>.
        /// </value>
        public bool HasReflectedFields
        {
            get
            {
                return !ReferenceEquals(this.currentFields, emptyFields);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type [is sealed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if the type [is sealed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSealed { get; internal set; }

        /// <summary>
        /// Gets the common use type value for the member type.
        /// </summary>
        public CommonUseType CommonUse { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a collection.
        /// </summary>
        public bool IsICollection { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether [is i enumerable].
        /// </summary>
        public bool IsIEnumerable { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether [is i dictionary].
        /// </summary>
        public bool IsIDictionary { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type [is abstract].
        /// </summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        public bool IsGeneric { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type [is an array].
        /// </summary>
        public bool IsArray { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type [is primitive or nullable or string].
        /// </summary>
        public bool IsPrimitiveOrStringOrNullable { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type [is primitive].
        /// </summary>
        public bool IsPrimitiveOrString { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type [is primitive nullable].
        /// </summary>
        public bool IsPrimitiveNullable { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type is an enumerated value.
        /// </summary>
        public bool IsEnum { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the type is a nullable enumerated value.
        /// </summary>
        public bool IsNullableEnum { get; internal set; }

        /// <summary>
        /// Reflects the fields.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>The current type description.</returns>
        public TypeDescription ReflectFields(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
        {
            Contract.Ensures(Contract.Result<TypeDescription>() != null);

            if (this.currentFieldBindings == bindings)
            {
                return this;
            }

            this.currentFieldBindings = bindings;

            FieldDescription[] value = this.fieldsCache[bindings] as FieldDescription[];
            if (value != null)
            {
                this.currentFields = value;
                return this;
            }

            value = this.ReflectedType.GetFields(bindings).ToFieldDescriptions();
            this.fieldsCache.Add(bindings, value);
            if (this.fieldsByName == null)
            {
                this.fieldsByName = value.ToHashtable();
            }
            else
            {
                for (int i = 0; i < value.GetLength(0); i++)
                {
                    var item = value[i];
                    if (this.fieldsByName.ContainsKey(item.MemberName))
                    {
                        continue;
                    }

                    this.fieldsByName.Add(value[i].MemberName, item);
                }
            }

            this.currentFields = value;
            return this;
        }

        /// <summary>
        /// Reflects the properties.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>The current type description.</returns>
        public TypeDescription ReflectProperties(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            Contract.Ensures(Contract.Result<TypeDescription>() != null);

            if (this.currentPropertyBindings == bindings)
            {
                return this;
            }

            this.currentPropertyBindings = bindings;

            PropertyDescription[] value = this.propertiesCache[bindings] as PropertyDescription[];
            if (value != null)
            {
                this.currentProperties = value;
                this.currentIndexedProperties = this.indexedPropertiesCache[bindings] as PropertyDescription[];
                return this;
            }

            var reflectedProperties = this.ReflectedType.GetProperties(bindings);
            var propertyDescriptions = new List<PropertyDescription>();
            var indexedPropertyDescriptions = new List<PropertyDescription>();

            if (this.propertiesByName == null)
            {
                this.propertiesByName = new Hashtable(StringComparer.Ordinal);
            }

            for (int i = 0; i < reflectedProperties.GetLength(0); i++)
            {
                var propertyInfo = reflectedProperties[i];
                var property = new PropertyDescription(propertyInfo, bindings);

                // Indexed properties are of the same name ("Item") but can be overloaded.
                if (property.IsIndexed)
                {
                    indexedPropertyDescriptions.Add(property);
                }
                else
                {
                    propertyDescriptions.Add(property);
                    if (!this.propertiesByName.ContainsKey(property.MemberName))
                    {
                        this.propertiesByName.Add(property.MemberName, property);
                    }
                }
            }

            value = propertyDescriptions.ToArray();
            var indexed = indexedPropertyDescriptions.ToArray();
            this.indexedPropertiesCache.Add(bindings, indexed);
            this.propertiesCache.Add(bindings, value);

            this.currentProperties = value;
            this.currentIndexedProperties = indexed;
            return this;
        }

        /// <summary>
        /// Reflects the methods.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>The current type description.</returns>
        public TypeDescription ReflectMethods(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            Contract.Ensures(Contract.Result<TypeDescription>() != null);

            if (this.currentMethodBindings == bindings)
            {
                return this;
            }

            this.currentMethodBindings = bindings;
            MethodDescription[] value = this.methodsCache[bindings] as MethodDescription[];
            if (value != null)
            {
                this.currentMethods = value;
                return this;
            }

            value = this.ReflectedType.GetMethods(bindings).ToMethodDescriptions();
            this.methodsCache.Add(bindings, value);

            if (this.methodsByName == null)
            {
                this.methodsByName = value.ToHashtable();
            }
            else
            {
                for (int i = 0; i < value.GetLength(0); i++)
                {
                    var key = new MethodKey(value[i].MemberName, value[i].GetParameterTypeHandles());
                    if (this.methodsByName.ContainsKey(key))
                    {
                        continue;
                    }

                    this.methodsByName.Add(key, value[i]);
                }
            }

            this.currentMethods = value;
            return this;
        }

        /// <summary>
        /// Reflects the ctor.
        /// </summary>
        /// <param name="ctorArgs">The ctor args.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns>True if a matching constructor is found, false otherwise.</returns>
        public bool ReflectCtor(Type[] ctorArgs, BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            var ctorArgHandles = ctorArgs.ToRuntimeHandles();
            Func<object[], object> ctor;
            return this.ReflectCtor(new MethodKey(ctorArgHandles), ctorArgHandles, bindings, out ctor);
        }

        /// <summary>
        /// Reflects the ctor.
        /// </summary>
        /// <param name="ctorArgHandles">The ctor args.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns>True if a matching constructor is found, false otherwise.</returns>
        public bool ReflectCtor(RuntimeTypeHandle[] ctorArgHandles, BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            Func<object[], object> ctor;
            return this.ReflectCtor(new MethodKey(ctorArgHandles), ctorArgHandles, bindings, out ctor);
        }

        /// <summary>
        /// Gets the full type information from the type handle.
        /// </summary>
        /// <returns>The full type information.</returns>
        public Type TypeFromHandle()
        {
            return this.ReflectedType;
        }

        /// <summary>
        /// Invokes the default constructor.
        /// </summary>
        /// <returns>The constructed instance.</returns>
        /// <exception cref="System.InvalidOperationException">The type {0} does not have a default constructor.</exception>
        public object CreateInstance()
        {
            if (this.defaultCtor == null)
            {
                if (!this.HasDefaultCtor)
                {
                    throw new InvalidOperationException(
                        string.Format("The type {0} does not have a default constructor.", this.ReflectedType.FullName));
                }

                this.defaultCtor = this.defaultCtorInfo.Value.ReflectConstructor();
            }

            return this.defaultCtor(null);
        }

        /// <summary>
        /// Creates the uninitialized instance using <see cref="FormatterServices.GetUninitializedObject(Type)"/>.
        /// </summary>
        /// <returns>The uninitialized object instance.</returns>
        /// <remarks>
        /// This implementation bypasses the call to the object constructor and is typically used for serialization scenarios.
        /// </remarks>
        public object CreateUninitializedInstance()
        {
            return FormatterServices.GetUninitializedObject(this.ReflectedType);
        }

        /// <summary>
        /// Creates the instance using the first constructor that matches the provided arguments.
        /// </summary>
        /// <param name="ctorArgs">The ctor args.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The target instance constructed with the provided arguments.
        /// </returns>
        public object CreateInstance(object[] ctorArgs, BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            var types = ctorArgs.ToRuntimeHandles();
            var key = new MethodKey(types);
            Func<object[], object> ctor;
            if (this.ReflectCtor(key, types, bindings, out ctor))
            {
                return ctor(ctorArgs);
            }

            return null;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The list of properties.
        /// </returns>
        public PropertyDescription[] GetProperties(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            if (this.currentPropertyBindings != bindings)
            {
                this.ReflectProperties(bindings);
            }

            return this.currentProperties;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The list of properties.
        /// </returns>
        public PropertyDescription[] GetIndexedProperties(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            if (this.currentPropertyBindings != bindings)
            {
                this.ReflectProperties(bindings);
            }

            return this.currentIndexedProperties;
        }

        /// <summary>
        /// Gets the property by name.
        /// </summary>
        /// <param name="propertyName">The name.</param>
        /// <returns>
        /// The property description or null if not found.
        /// </returns>
        public PropertyDescription GetProperty(string propertyName)
        {
            Contract.Requires(string.IsNullOrEmpty(propertyName) == false);

            var props = this.propertiesByName;
            if (props != null)
            {
                return props[propertyName] as PropertyDescription;
            }

            this.ReflectProperties();
            return this.propertiesByName[propertyName] as PropertyDescription;
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The list of fields.
        /// </returns>
        public FieldDescription[] GetFields(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
        {
            if (this.currentFieldBindings != bindings)
            {
                this.ReflectFields(bindings);
            }

            return this.currentFields;
        }

        /// <summary>
        /// Gets the field by name.
        /// </summary>
        /// <param name="fieldName">The name.</param>
        /// <returns>
        /// The field description or null if not found.
        /// </returns>
        public FieldDescription GetField(string fieldName)
        {
            Contract.Requires(string.IsNullOrEmpty(fieldName) == false);

            var fields = this.fieldsByName;
            if (fields != null)
            {
                return fields[fieldName] as FieldDescription;
            }

            this.ReflectFields();
            return this.fieldsByName[fieldName] as FieldDescription;
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>
        /// The list of methods.
        /// </returns>
        public MethodDescription[] GetMethods(BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            if (this.currentMethodBindings != bindings)
            {
                this.ReflectMethods(bindings);
            }

            return this.currentMethods;
        }

        /// <summary>
        /// Gets the method by name.
        /// </summary>
        /// <param name="methodName">The name.</param>
        /// <returns>
        /// The method description if matched, or null.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The methods must first be reflected before accessing a method by name.</exception>
        public MethodDescription GetMethod(string methodName)
        {
            RuntimeTypeHandle[] arguments = null;
            return this.GetMethod(methodName, arguments);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The method description if matched, or null.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The methods must first be reflected before accessing a method by name.</exception>
        public MethodDescription GetMethod(string methodName, params Type[] arguments)
        {
            return this.GetMethod(methodName, arguments.ToRuntimeHandles());
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="bindings">The bindings (optional).</param>
        /// <returns>
        /// The method description if matched, or null.
        /// </returns>
        /// <remarks>
        /// Getting the method by name will cache the method, but may result in a difference between all reflected methods 
        /// for the specified bindings and methods reflected by name.
        /// </remarks>
        public MethodDescription GetMethod(
            string methodName, 
            RuntimeTypeHandle[] arguments,
            BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            var key = new MethodKey(methodName, arguments);
            MethodDescription result = null;
            if (this.methodsByName != null)
            {
                result = this.methodsByName[key] as MethodDescription;
                if (result != null)
                {
                    return result;
                }
            }

            var methodInfo = arguments != null ?
                this.ReflectedType.GetMethod(methodName, bindings, Type.DefaultBinder, arguments.FromRuntimeHandles(), null) :
                this.ReflectedType.GetMethod(methodName, bindings);

            if (methodInfo == null)
            {
                return null;
            }

            result = new MethodDescription(methodInfo);
            if (this.methodsByName == null)
            {
                this.methodsByName = new Hashtable();
            }

            this.methodsByName.Add(key, result);
            return result;
        }

        /// <summary>
        /// Tries the get generic argument description.
        /// </summary>
        /// <param name="includedMembers">The included members.</param>
        /// <param name="bindings">The bindings.</param>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns>
        /// The type description or null.
        /// </returns>
        public TypeDescription TryGetGenericArgumentDescription(MemberTypes includedMembers, BindingFlags bindings, int argumentIndex = 0)
        {
            if (!this.IsGeneric)
            {
                return null;
            }

            var type = this.ReflectedType.TryGetGenericArgumentType(argumentIndex);
            if (type == null)
            {
                return null;
            }

            return type.GetTypeDescription(includedMembers, bindings);
        }

        /// <summary>
        /// Tries the get generic argument description.
        /// </summary>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns>
        /// The type description or null.
        /// </returns>
        public TypeDescription TryGetGenericArgumentDescription(int argumentIndex = 0)
        {
            if (!this.IsGeneric)
            {
                return null;
            }

            var type = this.ReflectedType.TryGetGenericArgumentType(argumentIndex);
            if (type == null)
            {
                return null;
            }

            return type.GetTypeDescription();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.TypeHandle.GetHashCode();
        }

        /// <summary>
        /// Compares the type description to a type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if the instances refer to the same type.</returns>
        public bool Equals(Type other)
        {
            if (other == null)
            {
                return false;
            }

            return this.TypeHandle.Equals(other.TypeHandle);
        }

        /// <summary>
        /// Initializes the default ctor.
        /// </summary>
        internal void InitializeDefaultCtor()
        {
            this.defaultCtor = this.defaultCtorInfo.Value.ReflectConstructor();
        }

        /// <summary>
        /// Reflects the ctor.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ctorArgHandles">The ctor argument handles.</param>
        /// <param name="bindings">The bindings.</param>
        /// <param name="ctor">The ctor method.</param>
        /// <returns>
        /// True if the constructor is reflected, false otherwise.
        /// </returns>
        private bool ReflectCtor(MethodKey key, RuntimeTypeHandle[] ctorArgHandles, BindingFlags bindings, out Func<object[], object> ctor)
        {
            Contract.Ensures(
                (Contract.Result<bool>() == true && Contract.ValueAtReturn(out ctor) != null) ||
                Contract.Result<bool>() == false);

            ctor = this.ctors[key] as Func<object[], object>;
            if (ctor != null)
            {
                return true;
            }

            var ctorArgs = ctorArgHandles.FromRuntimeHandles();
            var info = this.ReflectedType.GetConstructor(bindings, Type.DefaultBinder, ctorArgs, new ParameterModifier[] { });
            if (info != null)
            {
                ctor = info.ReflectConstructor();
                this.ctors.Add(key, ctor);
                return true;
            }
            else
            {
                return false;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.defaultCtorInfo != null);
            Contract.Invariant(this.ctors != null);
        }
    }
}
