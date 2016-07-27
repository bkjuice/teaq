using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Container class for building query batches.
    /// </summary>
    public class QueryBatch
    {
        /// <summary>
        /// The max allowed parameter count for a single batch fetch before the batch must be split.
        /// </summary>
        public const int MaxAllowedParameters = 2100;

        /// <summary>
        /// The max allowed statement count for a single batch fetch before the batch must be split.
        /// </summary>
        public const int MaxAllowedStatements = 999;

        /// <summary>
        /// The command list.
        /// </summary>
        private readonly List<QueryCommand> commands;

        /// <summary>
        /// The max batch size in statements.
        /// </summary>
        private readonly int maxBatchSize;

        /// <summary>
        /// The global parameters.
        /// </summary>
        private List<EmbeddedParameter> embeddedParameters;

        /// <summary>
        /// The global parameters.
        /// </summary>
        private List<SqlParameter> globalParameters;

        /// <summary>
        /// The result order, for reading.
        /// </summary>
        private List<Type> resultOrder;

        /// <summary>
        /// The global names.
        /// </summary>
        private HashSet<string> globalNames;

        /// <summary>
        /// The next fetch lower bound.
        /// </summary>
        private int nextLowerBound;

        /// <summary>
        /// The batch index to ensure unique parameter names.
        /// </summary>
        private int batchIndex;

        /// <summary>
        /// Value indicating the batch has global parameters that must be preserved 
        /// across split round trips, but have been used and must be copied.
        /// </summary>
        private bool mustCopyGlobalParameters;

        /// <summary>
        /// The current result
        /// </summary>
        private int currentResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBatch" /> class.
        /// </summary>
        /// <param name="batchChunkSize">Size of the batch chunk.</param>
        public QueryBatch(int batchChunkSize = 500)
        {
            Contract.Ensures(this.commands != null);

            if (batchChunkSize < 1)
            {
                batchChunkSize = 1;
            }
            else if (batchChunkSize > MaxAllowedStatements)
            {
                batchChunkSize = MaxAllowedStatements;
            }

            this.maxBatchSize = batchChunkSize;
            this.commands = new List<QueryCommand>(batchChunkSize * 2);
        }

        /// <summary>
        /// Gets the global parameter count.
        /// </summary>
        /// <value>
        /// The global parameter count.
        /// </value>
        public int GlobalParameterCount
        {
            get
            {
                if (this.globalParameters == null)
                {
                    return 0;
                }

                return this.globalParameters.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [global parameters exist].
        /// </summary>
        /// <value>
        /// <c>true</c> if [global parameters exist]; otherwise, <c>false</c>.
        /// </value>
        public bool GlobalParametersExist
        {
            get
            {
                if (this.embeddedParameters == null)
                {
                    return false;
                }

                return this.embeddedParameters.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the batch [has more queries].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has more queries]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasBatch
        {
            get
            {
                return this.nextLowerBound < this.commands.Count;
            }
        }

        /// <summary>
        /// Gets the index of the current batch.
        /// </summary>
        public int CurrentBatchIndex
        {
            get
            {
                return this.batchIndex;
            }
        }

        /// <summary>
        /// Gets the result set count.
        /// </summary>
        /// <value>
        /// The result set count.
        /// </value>
        public int ExpectedResultSetCount
        {
            get
            {
                if (this.resultOrder == null)
                {
                    return 0;
                }

                return this.resultOrder.Count;
            }
        }

        /// <summary>
        /// Gets the type of the current result.
        /// </summary>
        /// <value>
        /// The type of the current result.
        /// </value>
        public Type CurrentResult
        {
            get
            {
                if (this.resultOrder == null)
                {
                    return null;
                }

                if (this.resultOrder.Count < 1 || this.currentResult >= this.resultOrder.Count)
                {
                    return null;
                }

                return this.resultOrder[this.currentResult];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the batch has a next result.
        /// </summary>
        /// <returns>True if there is a next result, false otherwise.</returns>
        public bool HasResult
        {
            get
            {
                if (this.resultOrder == null)
                {
                    return false;
                }

                return this.currentResult < this.resultOrder.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating if the global name exists.
        /// </summary>
        /// <param name="parameterOrColumnName">Name of the parameter or source column.</param>
        /// <returns>True if the name exists, false otherwise.</returns>
        public bool GlobalNameExists(string parameterOrColumnName)
        {
            return this.globalNames != null && this.globalNames.Contains(parameterOrColumnName);
        }

        /// <summary>
        /// Adds the global parameter info for a parameter that is embedded in the query batch and not explicitly provided.
        /// </summary>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <param name="parameterName">Name of the embedded parameter.</param>
        public void AddEmbeddedParameter(string sourceColumnName, string parameterName)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(sourceColumnName) == false);
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(parameterName) == false);

            this.VerifyForParameterTracking(sourceColumnName, parameterName);
            var added = this.globalNames.Add(sourceColumnName);
            added = added | this.globalNames.Add(parameterName);
            if (added)
            {
                this.embeddedParameters.Add(
                    new EmbeddedParameter { SourceColumnName = sourceColumnName, ParameterName = parameterName });
            }
        }

        /// <summary>
        /// Adds the global parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void AddGlobalParameter(SqlParameter parameter)
        {
            Contract.Requires<ArgumentNullException>(parameter != null);

            this.VerifyForParameterTracking(parameter);            
            this.AddEmbeddedParameter(parameter.ParameterName, parameter.SourceColumn);
            this.globalParameters.Add(parameter);
        }

        /// <summary>
        /// Clears the global parameters and parameter info.
        /// </summary>
        public void ClearGlobals()
        {
            this.embeddedParameters = null;
            this.globalParameters = null;
            this.mustCopyGlobalParameters = false;
        }

        /// <summary>
        /// Moves the batch result pointer to the next result.
        /// </summary>
        /// <returns>True if there is a next result, false otherwise.</returns>
        internal bool MoveToNextResultType()
        {
            if (this.resultOrder == null)
            {
                return false;
            }

            this.currentResult++;
            return this.currentResult < this.resultOrder.Count;
        }

        /// <summary>
        /// Gets the next index for batch query parameters.
        /// </summary>
        /// <returns>The next batch indexer value.</returns>
        internal int NextBatchIndex()
        {
            return ++this.batchIndex;
        }

        /// <summary>
        /// Gets the global parameters.
        /// </summary>
        /// <returns>The array of global parameters.</returns>
        internal SqlParameter[] GlobalParameters()
        {
            lock (this)
            {
                if (this.globalParameters == null)
                {
                    return null;
                }

                if (!this.mustCopyGlobalParameters)
                {
                    this.mustCopyGlobalParameters = true;
                    return this.globalParameters.ToArray();
                }

                return this.globalParameters.Copy();
            }
        }

        /// <summary>
        /// Gets the next batch query command text.
        /// </summary>
        /// <returns>
        /// The batch command text.
        /// </returns>
        internal QueryCommand NextBatch()
        {
            int parameterCount;
            int commandTextLength;
            var max = this.GetNextBatchUpperBound(out parameterCount, out commandTextLength); 
            if (max == 0)
            {
                return QueryCommand.Empty;
            }

            var commandBuffer = StringBuilderCache.GetInstance(commandTextLength);
            var parameters = new FixedList<SqlParameter>(parameterCount);
            for (int i = this.nextLowerBound; i < this.commands.Count && i < max; i++)
            {
                var command = this.commands[i];
                commandBuffer.Append(command.CommandText);
                command.GetParameters()?.AppendTo(parameters);
            }

            this.nextLowerBound = max;
            this.GlobalParameters()?.AppendTo(parameters);

            var commandText = commandBuffer.ToString();
            StringBuilderCache.ReturnInstance(commandBuffer);
            return new QueryCommand(commandText, parameters.GetRawBuffer());
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="command">The command to add to the batch.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        internal void Add(QueryCommand command, bool canSplitBatch = true)
        {
            Contract.Requires(command != null);

            command.CanSplitBatch = canSplitBatch;
            this.commands.Add(command);
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <typeparam name="T">The type of entity to be read.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        internal void Add<T>(QueryCommand command, bool canSplitBatch = true)
        {
            this.Add(command, canSplitBatch);
            if (this.resultOrder == null)
            {
                this.resultOrder = new List<Type>(this.maxBatchSize);
            }

            this.resultOrder.Add(typeof(T));
        }

        /// <summary>
        /// Gets the global parameters.
        /// </summary>
        /// <returns>A read only collection of the current global parameter column and parameter names.</returns>
        internal List<EmbeddedParameter> EmbeddedParameters()
        {
            return this.embeddedParameters;
        }

        /// <summary>
        /// Finds the batch limit.
        /// </summary>
        /// <param name="parameterCount">The parameter count.</param>
        /// <param name="commandTextLength">Length of the command text.</param>
        /// <returns>
        /// The max batch size and parameter count.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The batch segment being processed exceeds the max allowed parameters of (MaxAllowedParameters: 2100) and cannot
        /// be processed. Ensure that at least one query command is set to be able to split the batch before this threshold 
        /// is reached by setting the 'CanSplitBatch' property to true.
        /// </exception>
        /// <remarks>
        /// The batch limit is constrained by SQL Server max allowed statements (999) and max allowed parameters (2100).
        /// This logic will probe for the parameter limit and if encountered, will attempt to set the max size of the batch
        /// to the last command that allows a batch split. If no such command exists, the bounds are exceeded
        /// and an exception is thrown.
        /// </remarks>
        private int GetNextBatchUpperBound(out int parameterCount, out int commandTextLength)
        {
            Contract.Ensures(Contract.Result<int>() >= 0);

            commandTextLength = 0;
            parameterCount = this.GlobalParameterCount;

            var lowerBound = this.nextLowerBound;
            var count = Math.Min(this.commands.Count, lowerBound + this.maxBatchSize);
            if (count == 0)
            {
                return 0;
            }

            int indexOfSplit = 0;
            
            // Ensure JIT compilation can safely eliminate bounds checking by checking the Count:  
            for (int i = lowerBound; i < this.commands.Count && i < count; i++)
            {
                var command = this.commands[i];
                if (parameterCount + command.ParameterCount > MaxAllowedParameters)
                {
                    if (indexOfSplit == 0)
                    {
                        ThrowWhenMaxParametersExceeded();
                    }

                    // Split the batch here. Ensure the pointer is positioned at the command after the split index 
                    // so the batch includes the "split'ed" command:
                    return indexOfSplit + 1;
                }

                parameterCount += command.ParameterCount;
                commandTextLength += command.CommandText.Length;
                if (command.CanSplitBatch)
                {
                    indexOfSplit = i;
                }
                else if (i + 1 == count)
                {
                    // Handle overflows where the batch cannot be split, and allow the max to increase:
                    if (count + 1 > MaxAllowedStatements)
                    {
                        ThrowWhenMaxStatementsExceeded();
                    }

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Throws when maximum number of parameters are exceeded and a batch cannot be split.
        /// </summary>
        private static void ThrowWhenMaxParametersExceeded()
        {
            throw new InvalidOperationException(
                $"The batch segment being processed exceeds the max allowed parameters of { MaxAllowedParameters }"
                + " and cannot be processed. Ensure that at least one query command is set to be able to split"
                + " the batch before this threshold is reached by setting the 'CanSplitBatch' property to true.");
        }

        /// <summary>
        /// Throws when maximum number of statements are exceeded and a batch cannot be split.
        /// </summary>
        private static void ThrowWhenMaxStatementsExceeded()
        {
            throw new InvalidOperationException(
                $"The query batch segment being processed exceeded the max allowed statements of { MaxAllowedStatements }"
                + " and cannot be processed. Ensure that at least one query command is set to be able to split" 
                + " the batch before this threshold is reached by setting the 'CanSplitBatch' property to true.");
        }

        /// <summary>
        /// Verifies the arguments and internal batch state for parameter tracking.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void VerifyForParameterTracking(SqlParameter parameter)
        {
            Contract.Requires(parameter != null);
            Contract.Requires(string.IsNullOrEmpty( parameter.ParameterName) == false);
            Contract.Requires(string.IsNullOrEmpty(parameter.SourceColumn) == false);

            this.VerifyForParameterTracking(parameter.SourceColumn, parameter.ParameterName);
            if (this.globalParameters == null)
            {
                this.globalParameters = new List<SqlParameter>();
            }
        }

        /// <summary>
        /// Verifies the arguments and internal batch state for parameter tracking.
        /// </summary>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        private void VerifyForParameterTracking(string sourceColumnName, string parameterName)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(sourceColumnName) == false);
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(parameterName) == false);

            if (this.globalNames == null)
            {
                this.globalNames = new HashSet<string>();
            }

            if (this.embeddedParameters == null)
            {
                this.embeddedParameters = new List<EmbeddedParameter>();
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.commands != null);
            Contract.Invariant(this.nextLowerBound >= 0);
            Contract.Invariant(this.GlobalParameterCount >= 0);
        }
    }
}
