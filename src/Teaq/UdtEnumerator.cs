using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of simple value types to be passed as a user defined table type to SQL Server.
    /// </summary>
    /// <typeparam name="T">The target value type to be enumerated.</typeparam>
    public abstract class UdtEnumerator<T> : IEnumerable<SqlDataRecord>
    {
        private IEnumerable<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtValueEnumerator{T}" /> class.
        /// </summary>
        /// <param name="items">The items to enumarate as a SQL Server user defined table.</param>
        protected UdtEnumerator(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            this.items = items;
        }

        /// <summary>
        /// Transforms the specified item to a <see cref="SqlDataRecord"/>.
        /// </summary>
        /// <param name="item">The item to transform.</param>
        /// <returns>The <see cref="SqlDataRecord"/> instance to use.</returns>
        protected abstract SqlDataRecord Transform(T item);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection of values as a user defined table parameter.
        /// </returns>
        public IEnumerator<SqlDataRecord> GetEnumerator()
        {
            return (items.Select(t => this.Transform(t))).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
