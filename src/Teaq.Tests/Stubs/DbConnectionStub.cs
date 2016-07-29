using System;
using System.Data;

namespace Teaq.Tests.Stubs
{
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
}
