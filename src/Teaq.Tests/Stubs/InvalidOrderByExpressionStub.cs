using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Tests.Stubs
{
    public class InvalidOrderByExpressionStub
    {
        public Expression<Func<IEnumerable<Customer>, IOrderedEnumerable<Customer>>> OrderByExpression { get; set; }

        public IOrderedEnumerable<Customer> UnknownMethod(IEnumerable<Customer> items)
        {
            return null;
        }

        public IOrderedEnumerable<Customer> OrderBy(IEnumerable<Customer> items)
        {
            return null;
        }
    }
}