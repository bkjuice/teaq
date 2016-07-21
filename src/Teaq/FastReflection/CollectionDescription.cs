using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght type description.
    /// </summary>
    public sealed class CollectionDescription : TypeDescription
    {
        /// <summary>
        /// The known read only collections from the .NET BCL.
        /// </summary>
        private static readonly HashSet<RuntimeTypeHandle> knownReadOnlyCollections = new HashSet<RuntimeTypeHandle> 
        {
            typeof(ReadOnlyCollectionBase).TypeHandle,
            typeof(ReadOnlyCollection<>).GetGenericTypeDefinition().TypeHandle,
            typeof(ReadOnlyObservableCollection<>).GetGenericTypeDefinition().TypeHandle
        };

        /// <summary>
        /// The default arguments for a collections "Add" method.
        /// </summary>
        private static readonly RuntimeTypeHandle[] defaultArgsForAdd = new RuntimeTypeHandle[] { typeof(object).TypeHandle };

        /// <summary>
        /// The collection add function.
        /// </summary>
        private Func<object, object[], object> addFunction;

        /// <summary>
        /// The collection add action.
        /// </summary>
        private Action<object, object[]> addAction;

        /// <summary>
        /// The add method flag (0 not resolved, 1 action, 2 func, -1 doesn't exist). Private, not enumerated.
        /// </summary>
        private int addMethodFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDescription" /> class without generating accessor IL.
        /// </summary>
        /// <param name="type">The type.</param>
        internal CollectionDescription(Type type)
            : base(type)
        {
            Contract.Requires(type != null);
        }

        /// <summary>
        /// Gets the collection item type.
        /// </summary>
        public TypeDescription ItemType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether a caller [can add items] to the collection.
        /// </summary>
        public bool CanAddItems 
        {
            get
            {
                return this.addMethodFlag != 0;
            }
        }

        /// <summary>
        /// Adds the item to the collection instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the collection Add method cannot be resolved.</exception>
        public void AddItem(object instance, object value)
        {
            if (this.addMethodFlag == 2)
            {
                if (this.CommonUse == CommonUseType.ListType)
                {
                    (instance as IList).Add(value);
                    return;
                }

                this.addFunction(instance, new object[] { value });
                return;
            }

            if (this.addMethodFlag == 0)
            {
                throw new InvalidOperationException("The collection does not have a viable add method.");
            }

            if (this.addMethodFlag == 1)
            {
                this.addAction(instance, new object[] { value });
                return;
            }
        }

        /// <summary>
        /// Resolves the add method.
        /// </summary>
        internal void ResolveAddMethod()
        {
            var baseType = this.ReflectedType;
            while (baseType != typeof(object) && baseType != null)
            {
                if (baseType.IsGenericType)
                {
                    baseType = baseType.GetGenericTypeDefinition();
                }

                if (knownReadOnlyCollections.Contains(baseType.TypeHandle))
                {
                    return;
                }

                baseType = baseType.BaseType;
            }

            if (this.CommonUse == CommonUseType.ListType)
            {
                this.addMethodFlag = 2;
                return;
            }

            var method = this.GetMethod("Add", new RuntimeTypeHandle[] { this.ItemType.TypeHandle });
            if (method == null)
            {
                method = this.GetMethod("Add", defaultArgsForAdd);
            }

            if (method == null)
            {
                return;
            }

            if (method.IsVoid)
            {
                this.addMethodFlag = 1;
                this.addAction = method.InvokeAction;
            }
            else
            {
                this.addMethodFlag = 2;
                this.addFunction = method.InvokeFunc;
            }
        }
    }
}
