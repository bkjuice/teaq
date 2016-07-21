using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Exception type thrown by the <see cref="Snowflake"/> ID generator.
    /// </summary>
    [Serializable]
    public class SnowflakeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeException"/> class.
        /// </summary>
        public SnowflakeException() : base()
        {
            this.ExceptionId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception
        /// is specified.
        /// </param>
        public SnowflakeException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.ExceptionId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SnowflakeException(string message) 
            : base(message)
        {
            this.ExceptionId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception
        /// being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or
        /// destination.
        /// </param>
        protected SnowflakeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Contract.Requires(info != null);

            this.ExceptionId = (Guid)info.GetValue("ExceptionId", typeof(Guid));
            this.HasBeenLogged = info.GetBoolean("HasBeenLogged");
        }

        /// <summary>
        /// Gets the exception identifier.
        /// </summary>
        public Guid ExceptionId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exception [has been logged].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has been logged]; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenLogged { get; set; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with 
        /// information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception
        /// being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or 
        /// destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Contract.Requires(info != null);

            info.AddValue("ExceptionId", this.ExceptionId);
            info.AddValue("HasBeenLogged", this.HasBeenLogged);
            base.GetObjectData(info, context);
        }
    }
}