using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.Stubs;

namespace Teaq.Tests.csUsage
{
    [TestClass]
    public class GettingStarted
    {
        public void CRUDCodeSnippets()
        {
            {
                var connectionString = "my db connection...";

                List<Customer> results;

                // Build a simple query using the fluent interface then execute it:
                using (var context = Repository.BuildContext(connectionString))
                {
                    var query = Repository.Default
                        .ForEntity<Customer>()
                        .BuildSelect()
                        .Where(c => c.CustomerId == 123)
                        .ToCommand();

                    // List<Customer>:
                    results = context.Query(query);
                }

                // do something with the results...

            }

            {
                // The following code defines a model for Customer. Models
                // are declarative, immutable once built, and threadsafe as singletons.
                // You can build any model for any scenario:

                var model = Repository.BuildModel(builder =>
                {
                    builder.Entity<Customer>()
                        .Column(c => c.CustomerId).IsIdentity()
                        .Column(c => c.CustomerKey).IsKey();
                });

                var customer = new Customer { CustomerKey = "SOME-CRM-KEY", /*...set customer data*/ };
                using (var context = Repository.BuildContext("some connection string"))
                {
                    var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
                    customer.CustomerId = context.ExecuteScalar<int>(command).GetValueOrDefault();
                }

                // customer is inserted, and identity is returned and assigned to the in memory PONO.
            }

            {
                var model = Repository.BuildModel(builder =>
                {
                    // This example shows mapping the table schema and name explicitly
                    // to the "crm" schema and "Customers" (plural) table:
                    builder.Entity<Customer>("crm", "Customers")
                        .Column(c => c.CustomerId).IsIdentity()
                        .Column(c => c.CustomerKey).IsKey();
                });

                var customer = new Customer { /*...set customer data*/ };
                using (var context = Repository.BuildContext("some connection string"))
                {
                    var command = model.ForEntity<Customer>()
                        .BuildUpdate(customer, c => c.CustomerId == 4096)
                        .ToCommand();

                    var rowsAffected = context.ExecuteNonQuery(command);
                }

            }


            {
                using (var context = Repository.BuildContext("some connection string"))
                {
                    var command = Repository.Default.ForEntity<Customer>()
                        .BuildDelete(c => c.CustomerId == 4096)
                        .ToCommand();

                    var rowsAffected = context.ExecuteNonQuery(command);
                }
            }

        }
    }
}
