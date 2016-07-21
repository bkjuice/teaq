using System;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Profiling.Scenarios
{
    internal class SimpleFluentStatementScenario
    {
        private readonly IDataModel model;

        private readonly Customer customer;

        public SimpleFluentStatementScenario(IDataModel model, Customer customer)
        {
            this.model = model;
            this.customer = customer;
        }

        public static Customer GetCustomerInstance()
        {
            return new Customer
            {
                CustomerId = 1,
                CustomerKey = "AB3FD321233",
                Inception = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }

        public bool Run()
        {
            return 
                this.BuildComplexSelectStatementWithModel(this.model) != null &&
                this.BuildInsertStatementWithModel(this.model, this.customer) != null;
        }

        public QueryCommand BuildComplexSelectStatementWithModel(IDataModel model)
        {
            return model.ForEntity<Customer>().BuildSelect().Where(c => c.CustomerId == 1 && c.CustomerKey != null)
                 .ToCommand();
        }

        public QueryCommand BuildInsertStatementWithModel(IDataModel model, Customer c)
        {
            return model.ForEntity<Customer>().BuildInsert(c).ToCommand();
        }
    }
}
