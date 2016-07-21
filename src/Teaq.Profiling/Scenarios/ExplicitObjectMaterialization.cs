using System;
using System.Data;
using System.Linq;

namespace Teaq.Profiling.Scenarios
{
    internal class ExplicitObjectMaterialization
    {
        private static readonly CustomerReader handler = new CustomerReader();

        public bool Run()
        {
            return
                this.Materialize100EntitiesUsingReadEntities() && this.Materialize100EntitiesUsingEnumerateEntities();
        }

        public bool Materialize100EntitiesUsingReadEntities()
        {
            var reader = CustomerData.OneHundredCustomers.GetReader();
            return reader.ReadEntities(handler).Count == 100;
        }

        public bool Materialize100EntitiesUsingEnumerateEntities()
        {
            var reader = CustomerData.OneHundredCustomers.GetReader();
            return reader.EnumerateEntities(handler).Count() == 100;
        }

        private class CustomerReader : DataHandler<Customer>
        {
            private int customerIdOrdinal;

            private int customerKeyOrdinal;

            private int changeOrdinal;

            private int inceptionOrdinal;

            private int modifiedOrdinal;


            public override void OnBeforeReading(IDataReader reader)
            {
                base.OnBeforeReading(reader);
                this.customerIdOrdinal = this.GetOrdinal("CustomerId");
                this.customerKeyOrdinal = this.GetOrdinal("CustomerKey");
                this.changeOrdinal = this.GetOrdinal("Change");
                this.inceptionOrdinal = this.GetOrdinal("Inception");
                this.modifiedOrdinal = this.GetOrdinal("Modified");
            }

            public override Customer ReadEntity(IDataReader reader)
            {
                // TODO: Profile testing for column existance...
                var customer = new Customer();
                if (this.customerIdOrdinal > -1)
                {
                    customer.CustomerId = reader.GetInt32(this.customerIdOrdinal);
                }

                if (this.customerKeyOrdinal > -1)
                {
                    customer.CustomerKey = reader.GetString(this.customerKeyOrdinal);
                }

                if (this.changeOrdinal > -1)
                {
                    customer.Change = reader.GetByte(this.changeOrdinal);
                }

                if (this.inceptionOrdinal > -1)
                {
                    customer.Inception = reader.GetDateTime(this.inceptionOrdinal);
                }

                if (this.modifiedOrdinal > -1)
                {
                    customer.Modified = (DateTimeOffset)reader.GetValue(this.modifiedOrdinal);
                }

                return customer;
            }
        }
    }
}
