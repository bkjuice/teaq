using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.Stubs;

namespace Teaq.Tests.csUsage
{
    [TestClass]
    public class Batching
    {
        private static string myConnection = ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString;

        [TestMethod]
        public void ExecuteABatchReadOperationUsingBatch()
        {
            using (var context = Repository.BuildBatchReader(myConnection, new QueryBatch()))
            {
                // Build your queries to be batch aware (must do).
                // These queries can be used in isolation, but are now part of the batch:
                Repository.Default
                        .ForEntity<Customer>()
                        .BuildSelect(top: 1)
                        .Where(c => c.CustomerId == 1)
                        .AddToBatch(context.QueryBatch);

                Repository.Default
                         .ForEntity<Address>()
                         .BuildSelect()
                         .Where(a => a.CustomerId == 1)
                         .AddToBatch(context.QueryBatch);

                context.NextResult();
                var customer = context.ReadEntitySet<CustomerWithAddresses>().First();
                if (context.NextResult())
                {
                    customer.Addresses = context.ReadEntitySet<Address>();
                }

                // Return the customer...
            }
        }
    }
}
