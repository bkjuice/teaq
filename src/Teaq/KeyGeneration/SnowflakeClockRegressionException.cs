﻿using System;
using System.Runtime.Serialization;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Thrown when the <see cref="Snowflake"/> ID generator encountes a backwards clock movement that is beyond its tolerance to wait.
    /// </summary>
    [Serializable]
    public class SnowflakeClockRegressionException : SnowflakeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeClockRegressionException"/> class.
        /// </summary>
        public SnowflakeClockRegressionException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeClockRegressionException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner 
        /// exception is specified.
        /// </param>
        public SnowflakeClockRegressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeClockRegressionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SnowflakeClockRegressionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeClockRegressionException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception 
        /// being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source 
        /// or destination.
        /// </param>
        protected SnowflakeClockRegressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}