using System.Data.SqlClient;
using Teaq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Data structure to encapsulate a query batch command text and parameters.
    /// </summary>
    public class QueryCommand
    {
        /// <summary>
        /// The empty command.
        /// </summary>
        internal static readonly QueryCommand Empty = new QueryCommand();

        /// <summary>
        /// The parameters
        /// </summary>
        private readonly SqlParameter[] parameters;

        /// <summary>
        /// Indicates the query parameters are used and must be copied.
        /// </summary>
        private bool parametersUsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        public QueryCommand(string commandText, bool canSplitBatch = true) : this(commandText, null, canSplitBatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        public QueryCommand(string commandText, SqlParameter[] parameters, bool canSplitBatch = true)
            : this()
        {
            this.CommandText = commandText;
            this.parameters = parameters;
            this.CanSplitBatch = canSplitBatch;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand"/> class.
        /// </summary>
        internal QueryCommand()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the command is empty.
        /// </summary>
        public bool IsEmpty 
        {
            get
            {
                return string.IsNullOrEmpty(this.CommandText); 
            }
        }

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <value>
        /// The parameter count.
        /// </value>
        public int ParameterCount
        {
            get
            {
                return this.parameters != null ? this.parameters.GetLength(0) : 0;
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        public string CommandText { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can be split in batch.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be split in batch; otherwise, <c>false</c>.
        /// </value>
        public bool CanSplitBatch { get; set; }

        /// <summary>
        /// Gets or sets the model used to generate the query, if applicable.
        /// </summary>
        public IDataModel Model { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>The array of parameters.</returns>
        public SqlParameter[] GetParameters()
        {
            if (!this.parametersUsed)
            {
                this.parametersUsed = true;
                return this.parameters;
            }

            return this.parameters.Copy();
        }
    }
}
