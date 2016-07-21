using System;
using System.Diagnostics.Contracts;
using Teaq.FastReflection;

namespace Teaq
{
    internal struct DataFieldSetter
    {
        public DataFieldSetter(PropertyDescription property) : this(property, null)
        {
            Contract.Requires(property != null);
        }

        public DataFieldSetter(PropertyDescription property, Func<object, object> converter) : this()
        {
            Contract.Requires(property != null);

            this.Property = property;
            this.Converter = converter;
        }

        public PropertyDescription Property;

        public Func<object, object> Converter;

        public void SetValue(object entity, object value)
        {
            Contract.Requires(this.Property != null);

            this.Property.SetValue(entity, this.Converter?.Invoke(value) ?? value);
        }
    }
}
