using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    public interface IAbstractEntity : ICommonAbstractEntity
    {
        new int Id { get; set; }

        string Data { get; set; }
    }

    public interface ICommonAbstractEntity
    {
        int Id { get; set; }
    }

    public class DataHandlerStub<TEntity> : DataHandler<TEntity> where TEntity: class
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

    public class ConcreteEntity : IAbstractEntity
    {
        public int Id { get; set; }

        public string Data { get; set; }
    }

    public class NonIListCollection<T> : IEnumerable<T>
    {
        private List<T> items;

        public NonIListCollection(List<T> items)
        {
            this.items = items;
        }

        public bool InvalidContains(T item)
        {
            return items.Contains(item);
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }

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

    public class InvalidJoinExpressionStub
    {
        public Expression<Func<Customer, Address, bool>> OnExpression { get; set; }

        public bool MethodExpression(Customer c, Address a)
        {
            return false;
        }
    }

    internal class QueryBuilderStub<T> : QueryBuilder<T>
    {
        public QueryBuilderStub() : base() { }

        public QueryBuilderStub(IDataModel dataModel) : base(dataModel) { }

        public IDataModel VerifiableDataModel
        {
            get
            {
                return this.DataModel;
            }
        }

        public static string VerifiableCheckColumns(string tableAlias, Type targetType, IEntityConfiguration config, params string[] selectColumnList)
        {
            return targetType.FormatColumns(tableAlias, config, selectColumnList);
        }

        public static string VerifiableFormatColumns(string tableAlias, Type targetType, IEntityConfiguration config, params string[] selectColumnList)
        {
            return targetType.FormatColumns(tableAlias, config, selectColumnList);
        }

        public static bool VerifiableColumnInScope(string propertyName, Type propertyType, QueryType queryType, IEntityConfiguration config = null)
        {
            var scopeFunc = queryType.ScopeFunc();
            return scopeFunc(propertyType.TypeHandle, propertyName, config);
        }
    }

    public class DBParameterCollectionStub : IDataParameterCollection
    {
        private List<IDataParameter> parameters = new List<IDataParameter>();

        public int Count
        {
            get { return this.parameters.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return new object(); }
        }

        public object this[string parameterName]
        {
            get
            {
                return this.parameters.Select(p => p.ParameterName == parameterName).FirstOrDefault();
            }

            set
            {
                var index = this.parameters.FindIndex(p => p.ParameterName == parameterName);
                if (index > 0)
                {
                    this.parameters[index] = value as IDataParameter;
                }
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public object this[int index]
        {
            get
            {
                return this.parameters[index];
            }

            set
            {
                this.parameters[index] = value as IDataParameter;
            }
        }

        public bool Contains(string parameterName)
        {
            return this.parameters.Select(p => p.ParameterName == parameterName).Any();
        }

        public int IndexOf(string parameterName)
        {
            return this.parameters.FindIndex(p => p.ParameterName == parameterName);
        }

        public void RemoveAt(string parameterName)
        {
            var index = this.parameters.FindIndex(p => p.ParameterName == parameterName);
            if (index > 0)
            {
                this.parameters.RemoveAt(index);
            }
        }
      
        public int Add(object value)
        {
            var nextIndex = this.parameters.Count;
            this.parameters.Add(value as IDataParameter);
            return nextIndex;
        }

        public void Clear()
        {
            this.parameters.Clear();
        }

        public bool Contains(object value)
        {
            return this.parameters.Contains(value as IDataParameter);
        }

        public int IndexOf(object value)
        {
            return this.parameters.IndexOf(value as IDataParameter);
        }

        public void Insert(int index, object value)
        {
            this.parameters.Insert(index, value as IDataParameter);
        }

        public void Remove(object value)
        {
            this.parameters.Remove(value as IDataParameter);
        }

        public void RemoveAt(int index)
        {
            this.parameters.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            Array.Copy(this.parameters.ToArray(), 0, array, index, this.parameters.Count);
        }

        public IEnumerator GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }
    }

    public class DbCommandStub : IDbCommand
    {
        private IDataParameterCollection parameters = new DBParameterCollectionStub();

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDataHelper DataHelper { get; set; }

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }
       

        public IDataParameterCollection Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public Func<int> ExecuteNonQueryCallback { get; set; }

        public Func<CommandBehavior, IDataReader> ExecuteReaderWithBehaviorCallback{ get; set; }

        public Func<IDataReader> ExecuteReaderCallback  { get; set; }

        public Func<object> ExecuteScalarCallback  { get; set; }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery()
        {
            if (this.ExecuteNonQueryCallback != null)
            {
                return this.ExecuteNonQueryCallback();
            }

            return -1;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (this.ExecuteReaderWithBehaviorCallback != null)
            {
                return this.ExecuteReaderWithBehaviorCallback(behavior);
            }

            return this.CreateDefaultReader();
        }

        public IDataReader ExecuteReader()
        {
            if (this.ExecuteReaderCallback != null)
            {
                return this.ExecuteReaderCallback();
            }

            return this.CreateDefaultReader();
        }

        public object ExecuteScalar()
        {
            if (this.ExecuteScalarCallback != null)
            {
                return this.ExecuteScalarCallback();
            }

            return new object();
        }

        public void Cancel()
        {
        }

        public void Prepare()
        {
        }

        public void Dispose()
        {
        }

        private IDataReader CreateDefaultReader()
        {
            if (this.DataHelper != null)
            {
                return this.DataHelper.GetReader();
            }

            return new TableHelper().GetReader();
        }
    }

    public class DbConnectionStub : IDbConnection
    {
        public IDbTransaction MockTransaction { get; set; }

        public IDbCommand MockCommand { get; set; }

        public bool CloseInvoked { get; private set; }
        
        public bool DisposeInvoked { get; private set; }
        
        public bool OpenInvoked { get; private set; }

        public bool BeginTransactionInvoked { get; private set; }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            this.BeginTransactionInvoked = true;
            return this.MockTransaction;
        }

        public IDbTransaction BeginTransaction()
        {
            this.BeginTransactionInvoked = true;
            return this.MockTransaction;
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            this.CloseInvoked = true;
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; set; }

        public IDbCommand CreateCommand()
        {
            if (this.MockCommand == null)
            {
                this.MockCommand = new DbCommandStub();
            }

            return this.MockCommand;
        }

        public string Database { get; set; }

        public void Open()
        {
            this.OpenInvoked = true;
        }

        public ConnectionState State { get; set; }

        protected virtual void Dispose(Boolean flag)
        {
            this.DisposeInvoked = true;
            this.Close();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }

    public class Address
    {
        public int AddressId { get; set; }

        public int CustomerId { get; set; }

        public byte Change { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }
    }

    public class CustomerWithNullable
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }

    public class CustomerWithNullableId
    {
        public int CustomerId { get; set; }

        public int? RelatedCustomerId { get; set; }
    }

    public class CustomerWithNestedProperty
    {
        public CustomerWithNestedProperty()
        {
            this.Keys = new NestedKeys();
        }

        public NestedKeys Keys { get; private set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }

    public class CustomerWithAddress
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public Address Address { get; set; } 
    }

    public class CustomerWithAddresses
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public List<Address> Addresses { get; set; }
    }


    public class NestedKeys
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }
    }

    public class UserTenancy
    {
        public string UserName { get; set; }

        public string TenancyKey { get; set; }
    }
}
