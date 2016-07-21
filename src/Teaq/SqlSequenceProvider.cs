using System;
using System.Data;
using System.Data.SqlClient;
using Teaq.KeyGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementation to support interacting with an SQL Server SEQUENCE function.
    /// </summary>
    /// <typeparam name="T">The SEQUENCE function type, typically <see cref="Int32"/> or <see cref="Int64"/>.</typeparam>
    public class SqlSequenceProvider<T> : SequenceProvider<T> where T : struct
    {
        /// <summary>
        /// The connection to the SQL server hosting the trunk instance.
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// The working SQL command.
        /// </summary>
        private SqlCommand command;

        /// <summary>
        /// The output parameter.
        /// </summary>
        private SqlParameter output;

        /// <summary>
        /// The range.
        /// </summary>
        private SqlParameter range;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSequenceProvider{T}" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        public SqlSequenceProvider(string connection, string sequenceName)
        {
            this.connection = new SqlConnection(connection);
            this.command = new SqlCommand("sys.sp_sequence_get_range");
            this.command.CommandType = CommandType.StoredProcedure;
            this.command.Parameters.AddWithValue("@sequence_name", sequenceName);

            this.range = new SqlParameter("@range_size", 10);
            this.output = new SqlParameter("@range_first_value", SqlDbType.Variant);
            this.output.Direction = ParameterDirection.Output;

            this.command.Parameters.Add(this.range);
            this.command.Parameters.Add(this.output);

            this.command.Connection = this.connection;
            try
            {
                this.connection.Open();
                this.command.Prepare();
            }
            finally
            {
                if (this.connection.State != ConnectionState.Closed)
                {
                    this.connection.Close();
                }
            }
        }

        /// <summary>
        /// Reserves the next available range.
        /// </summary>
        /// <param name="rangeSize">Size of the range.</param>
        /// <returns>The first value in the reserved range.</returns>
        public override T Reserve(int rangeSize)
        {
            this.range.Value = rangeSize;
            this.connection.Open();
            this.command.ExecuteNonQuery();
            this.connection.Close();
            return (T)this.output.Value;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.command.Dispose();
                this.connection.Dispose();
            }

            base.Dispose(true);
        }
    }
}