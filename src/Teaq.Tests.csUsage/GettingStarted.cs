using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.Stubs;

namespace Teaq.Tests.csUsage
{
    [TestClass]
    public class GettingStarted
    {
        [TestMethod]
        public void CodeSnippets()
        {
            var connectionString = "...";

            // Build a simple query using the fluent interface then execute it:
            using (var context = Repository.BuildContext(connectionString))
            {
                var query = Repository.Default
                    .ForEntity<Customer>()
                    .BuildSelect()
                    .Where(c => c.CustomerId == 123)
                    .ToCommand();

                // List<Customer>:
                var results = context.Query(query);
            }



        }
    }
}
