using System;
using System.Collections.Generic;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Profiling.Scenarios
{
    internal class BatchFluentStatementScenario
    {
        private readonly IDataModel model;

        private readonly Customer customer;

        public BatchFluentStatementScenario(IDataModel model, Customer customer)
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
            var batch = new QueryBatch();
            this.BuildComplexSelectStatementWithModel(this.model, batch);
            this.BuildComplexSelectStatementWithModelAndColumns(this.model, batch);
            this.BuildInsertStatementWithModel(this.model, this.customer, batch);
            this.BuildComplexSelectStatementWithModelUsingContains(this.model, batch);
            return batch.NextBatch() != null;
        }

        public QueryCommand BuildComplexSelectStatementWithModel(IDataModel model, QueryBatch batch)
        {
            return model.ForEntity<Customer>().BuildSelect().Where(c => c.CustomerId == 1 && c.CustomerKey != null)
                 .AddToBatch(batch);
        }

        public QueryCommand BuildComplexSelectStatementWithModelUsingContains(IDataModel model, QueryBatch batch)
        {
            var ids = new List<int> { 1, 2, 4, 5 };
            return model.ForEntity<Customer>().BuildSelect().Where(c => ids.Contains( c.CustomerId))
                 .AddToBatch(batch);
        }

        public QueryCommand BuildComplexSelectStatementWithModelAndColumns(IDataModel model, QueryBatch batch)
        {
            return model.ForEntity<Customer>().BuildSelect("CustomerId", "Change", "FirstName", "LastName").Where(c => c.CustomerId == 1 && c.CustomerKey != null)
                 .AddToBatch(batch);
        }

        public QueryCommand BuildInsertStatementWithModel(IDataModel model, Customer c, QueryBatch batch)
        {
            return model.ForEntity<Customer>().BuildInsert(c).AddToBatch(batch);
        }
    }
}
