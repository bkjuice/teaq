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

    public class DataHandlerStub<TEntity> : DataHandler<TEntity> where TEntity : class
    {
        private int resultCount;

        public int TestableEstimatedRowCount { get; set; }

        public Func<int, TEntity> Builder { get; set; }

        public Func<int, bool> CanReadBehavior { get; set; }

        protected override int EstimatedRowCount
        {
            get
            {
                return this.TestableEstimatedRowCount;
            }
        }

        public int BaseEstimatedRowCount { get { return base.EstimatedRowCount; } }

        public override TEntity ReadEntity(IDataReader reader)
        {
            resultCount++;
            if (this.Builder != null)
            {
                return this.Builder(resultCount - 1);
            }

            return default(TEntity);
        }
    }
}