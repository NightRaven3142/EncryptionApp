﻿using System;

namespace FactaLogicaSoftware.CryptoTools.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an invalid key length
    /// </summary>
    [Serializable]
    public class InvalidKeyLengthException : BadKeyException
    {
        /// <inheritdoc />
        /// <summary>
        /// The default constructor
        /// </summary>
        public InvalidKeyLengthException() : base("Key is not valid")
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// The constructor which defines a message to be carried by the
        /// exception
        /// </summary>
        /// <param name="message">The string to be the message</param>
        public InvalidKeyLengthException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// The constructor which defines a message to be carried by the
        /// exception and the inner exception to base it off
        /// </summary>
        /// <param name="message">The string to be the message</param>
        /// <param name="inner">The inner exception</param>
        public InvalidKeyLengthException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}