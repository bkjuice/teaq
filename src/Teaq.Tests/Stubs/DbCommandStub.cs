using System;
using System.Data;

namespace Teaq.Tests.Stubs
{
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

        public Func<CommandBehavior, IDataReader> ExecuteReaderWithBehaviorCallback { get; set; }

        public Func<IDataReader> ExecuteReaderCallback { get; set; }

        public Func<object> ExecuteScalarCallback { get; set; }

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
}
