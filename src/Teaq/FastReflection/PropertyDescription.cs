using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Teaq.FastReflection
{
    /// <summary>
    /// Lightwieght property descriptor.
    /// </summary>
    public sealed class PropertyDescription : MemberDescription
    {
        /// <summary>
        /// Gets the get value method.
        /// </summary>
        private readonly Func<object, object> getValueFunc;

        /// <summary>
        /// Gets the get value method.
        /// </summary>
        private readonly Func<object, object, object> indexedGetValueFunc;

        /// <summary>
        /// The index types if the property is indexed.
        /// </summary>
        private readonly Type[] indexParamenterTypes;

        /// <summary>
        /// Gets the set value method.
        /// </summary>
        private Action<object, object> setValueFunc;

        /// <summary>
        /// Gets the set value method.
        /// </summary>
        private Action<object, object[]> indexedSetValueFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescription" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="bindings">The bindings.</param>
        internal PropertyDescription(PropertyInfo info, BindingFlags bindings)
            : base(info, info.PropertyType)
        {
            Contract.Requires(info != null);

            this.PropertyType = info.PropertyType;
            this.PropertyTypeHandle = info.PropertyType.TypeHandle;
            var indexParameters = info.GetIndexParameters();

            var nonpublic = bindings.HasFlag(BindingFlags.NonPublic);
            if (indexParameters.GetLength(0) == 0)
            {
                this.getValueFunc = info.GetGetMethod(nonpublic).ReflectGetter();
                this.setValueFunc = info.GetSetMethod(nonpublic)?.ReflectSetter(info.PropertyType);
                this.indexedGetValueFunc = (e, o) => { throw new InvalidOperationException("The indexed getter is not available. The property is not indexed."); };
            }
            else
            {
                this.getValueFunc = (e) => { throw new InvalidOperationException("The getter is not available. The property is indexed."); };
                this.indexParamenterTypes = indexParameters.Select(p => p.ParameterType).ToArray();
                this.indexedGetValueFunc = info.GetGetMethod(nonpublic).ReflectIndexedGetter();
                this.indexedSetValueFunc = info.GetSetMethod(nonpublic)?.ReflectIndexedSetter();
                this.IsIndexed = true;
            }

            this.MemberType = info.PropertyType.GetTypeDescription();
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets the property type handle.
        /// </summary>
        public RuntimeTypeHandle PropertyTypeHandle { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.setValueFunc == null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is indexed.
        /// </summary>
        public bool IsIndexed { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// The property value.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the getter is not available. Use the overload that accepts the index value.
        /// </exception>
        public object GetValue(object instance)
        {
            return this.getValueFunc(instance);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="indexer">The indexer.</param>
        /// <returns>
        /// The property value.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the getter is not available. Use the overload that does not require an index value.
        /// </exception>
        public object GetValue(object instance, object indexer)
        {
            return this.indexedGetValueFunc(instance, indexer);
        }

        /// <summary>
        /// Gets the set value method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The set value.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the setter is indexed. Use the overload that accepts the index value.
        /// </exception>
        public void SetValue(object instance, object value)
        {
            // This is often considered an anti-pattern, however
            // profiling indicates checking for null is slower and 
            // this is a very hot path:
            try
            {
                this.setValueFunc(instance, value);
            }
            catch (NullReferenceException e)
            {
                throw new InvalidOperationException(
                    $"The setter is not reflected for {this.MemberName} in {this.DeclaringType}. Ensure the reflection bindings include private and base class access and this property is not an indexed property.",
                    e);
            }
        }

        /// <summary>
        /// Gets the set value method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <param name="indexer">The indexer.</param>
        /// <returns>
        /// The set value.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the setter is not indexed or if the setter is not reflected.
        /// </exception>
        public void SetValue(object instance, object value, object indexer)
        {
            try
            {
                this.indexedSetValueFunc(instance, new object[] { indexer, value });
            }
            catch (NullReferenceException e)
            {
                throw new InvalidOperationException(
                 $"The setter is not reflected for {this.MemberName} in {this.DeclaringType}. Ensure the reflection bindings include all expected properties and this property is indexed.",
                 e);
            }
        }

        /// <summary>
        /// Gets the index parameter types.
        /// </summary>
        /// <returns>The array of index parameter types, so callers can identify overloaded indexed properties.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the setter is not indexed or if the setter is not reflected.
        /// </exception>
        public Type[] GetIndexParameterTypes()
        {
            if (!this.IsIndexed)
            {
                throw new InvalidOperationException(
                    "The property is not indexed. Check the 'IsIndexed' property before testing the index parameter types.");
            }

            return this.indexParamenterTypes;
        }

        /// <summary>
        /// Ensures the setter is available and returns false if it isn't able to be reflected.
        /// </summary>
        /// <param name="bindings">The bindings to use when reflecting the setter.</param>
        /// <returns>True if the setter exists and has been reflected; false otherwise.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the property could not be reflected using the provided bindings.
        /// </exception>
        public bool TryEnsureSetter(BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (this.setValueFunc != null)
            {
                return true;
            }

            var info = this.IsIndexed ?
                Type.GetTypeFromHandle(
                    this.DeclaringTypeHandle).GetProperty(this.MemberName, bindings, null, this.PropertyType, this.indexParamenterTypes, null) :
                Type.GetTypeFromHandle(
                    this.DeclaringTypeHandle).GetProperty(this.MemberName, bindings);

            if (info == null)
            {
                throw new InvalidOperationException(
                    "The property for member name {0} could not be reflected using the provided bindings.".ToFormat(this.MemberName));
            }

            var setter = info.GetSetMethod(true);
            if (setter == null)
            {
                return false;
            }

            if (!this.IsIndexed)
            {
                this.setValueFunc = setter.ReflectSetter(this.PropertyType);
            }
            else
            {
                this.indexedSetValueFunc = setter.ReflectIndexedSetter();
            }

            return true;
        }

        /// <summary>
        /// Ensures the setter is available and throws if it isn't able to be reflected.
        /// </summary>
        /// <param name="bindings">The bindings to use when reflecting the setter.</param>
        public void EnsureSetter(BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (!this.TryEnsureSetter(bindings))
            {
                if (!this.IsIndexed)
                {
                    throw new InvalidOperationException(
                        "The property does not have a setter for property named {0}.".ToFormat(this.MemberName));
                }
                else
                {
                    throw new InvalidOperationException(
                        "The property does not have a setter for indexed property with arguments of type {0}."
                            .ToFormat(string.Join<Type>(", ", this.indexParamenterTypes)));
                }
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.PropertyType != null);
            Contract.Invariant(this.PropertyTypeHandle != null);
            Contract.Invariant(this.getValueFunc != null);
            Contract.Invariant(this.indexedGetValueFunc != null);
        }
    }
}