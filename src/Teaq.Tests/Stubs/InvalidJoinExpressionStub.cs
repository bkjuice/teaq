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
    public class InvalidJoinExpressionStub
    {
        public Expression<Func<Customer, Address, bool>> OnExpression { get; set; }

        public bool MethodExpression(Customer c, Address a)
        {
            return false;
        }
    }
}