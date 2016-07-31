using System;
using System.Data;
using System.Diagnostics.Contracts;

namespace Teaq
{
    internal class DelegatingReaderHandler<T> : DataHandler<T>
    {
        private Func<IDataReader, T> innerHandler;

        public DelegatingReaderHandler(Func<IDataReader, T> innerHandler)
        {
            Contract.Requires(innerHandler != null);

            this.innerHandler = innerHandler;
        }

        public override T ReadEntity(IDataReader reader)
        {
            return this.innerHandler(reader);
        }
    }
}
