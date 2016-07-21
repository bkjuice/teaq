using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght method type description.
    /// </summary>
    public sealed class MethodDescription : MemberDescription
    {
        /// <summary>
        /// The argument type handles
        /// </summary>
        private readonly RuntimeTypeHandle[] parameterTypeHandles;

        /// <summary>
        /// Invokes the method if the return type is void.
        /// </summary>
        private readonly Action<object, object[]> invokeAction;

        /// <summary>
        /// Invokes the method if the return type is not void.
        /// </summary>
        private readonly Func<object, object[], object> invokeFunc;

        /// <summary>
        /// The return type handle
        /// </summary>
        private readonly RuntimeTypeHandle returnTypeHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDescription" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        internal MethodDescription(MethodInfo info)
            : base(info, info.ReturnType)
        {
            Contract.Requires(info != null);

            this.returnTypeHandle = info.ReturnType.TypeHandle;
            this.parameterTypeHandles = info.GetParameters().ToRuntimeHandles();
            if (info.ReturnType == typeof(void))
            {
                this.IsVoid = true;
                this.invokeAction = info.ReflectAction();
                this.invokeFunc = (e, a) =>
                {
                    throw new InvalidOperationException("The method returns void. Call InvokeAction instead. Check for this condition using the IsVoid property.");
                };
            }
            else
            {
                this.invokeFunc = info.ReflectFunc();
                this.invokeAction = (e, a) =>
                {
                    throw new InvalidOperationException("The method returns a value. Call InvokeFunc instead. Check for this condition using the IsVoid property.");
                };
            }

            this.MemberType = info.ReturnType.GetTypeDescription();
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        public Type ReturnType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);

                return Type.GetTypeFromHandle(this.returnTypeHandle);
            }
        }

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <value>
        /// The parameter count.
        /// </value>
        public int ParameterCount
        {
            get
            {
                return this.parameterTypeHandles.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is void.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is void; otherwise, <c>false</c>.
        /// </value>
        public bool IsVoid { get; private set; }

        /// <summary>
        /// Invokes the method if the return type is void.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the method returns a value. Check for this condition using the IsVoid property and if true, call InvokeFunc instead. 
        /// </exception>
        public void InvokeAction(object target, object[] parameters)
        {
            this.invokeAction(target, parameters);
        }

        /// <summary>
        /// Gets the method if the return type is not void.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The function result.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The method returns void. Call InvokeAction instead. Check for this condition using the IsVoid property.
        /// </exception>
        public object InvokeFunc(object target, object[] parameters)
        {
            return this.invokeFunc(target, parameters);
        }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <returns>The list of parameter types.</returns>
        public Type[] GetParameterTypes()
        {
            return this.parameterTypeHandles.FromRuntimeHandles();
        }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <returns>The list of parameter types.</returns>
        public RuntimeTypeHandle[] GetParameterTypeHandles()
        {
            Contract.Ensures(Contract.Result<RuntimeTypeHandle[]>() != null);

            return this.parameterTypeHandles;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.parameterTypeHandles != null);
            Contract.Invariant(this.invokeAction != null);
            Contract.Invariant(this.invokeFunc != null);
        }
    }
}
