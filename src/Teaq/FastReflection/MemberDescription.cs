using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Teaq.FastReflection
{
    /// <summary>
    /// The target member description.
    /// </summary>
    public abstract class MemberDescription
    {
        /// <summary>
        /// The attributes associated with the member.
        /// </summary>
        private readonly CustomAttributeData[] attributeData;

        /// <summary>
        /// The attributes associated with the type indexed by name.
        /// </summary>
        private readonly HashSet<string> attributeDataByName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDescription" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="typeOfMember">The type of member.</param>
        internal MemberDescription(MemberInfo info, Type typeOfMember)
        {
            Contract.Requires(info != null);

            this.MemberName = info.Name;
            this.ReflectedType = typeOfMember;

            if (info.DeclaringType != null)
            {
                this.DeclaringTypeHandle = info.DeclaringType.TypeHandle;
                this.DeclaringType = info.DeclaringType;
            }

            this.attributeData = info.GetCustomAttributesData().ToArray();
            this.attributeDataByName = new HashSet<string>(
                this.attributeData.Select(i => i.Constructor.DeclaringType.Name), StringComparer.Ordinal);
        }
       
        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string MemberName { get; private set; }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type ReflectedType { get; private set; }

        /// <summary>
        /// Gets the declaring type handle.
        /// </summary>
        public RuntimeTypeHandle DeclaringTypeHandle { get; private set; }

        /// <summary>
        /// Gets or sets the member type description.
        /// </summary>
        public TypeDescription MemberType { get; protected set; }

        /// <summary>
        /// Gets the attribute data.
        /// </summary>
        /// <returns>The array of custom attribute data applied to the type.</returns>
        public CustomAttributeData[] GetAttributeData()
        {
            Contract.Ensures(Contract.Result<CustomAttributeData[]>() != null);

            return this.attributeData;
        }

        /// <summary>
        /// Gets a value indicating if the attribute is applied to the member.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The custom attribute data for the specified attribute name.
        /// </returns>
        public bool AttributeIsDefined(string name)
        {
            return this.attributeDataByName.Contains(name);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.attributeDataByName != null);
        }
    }
}
