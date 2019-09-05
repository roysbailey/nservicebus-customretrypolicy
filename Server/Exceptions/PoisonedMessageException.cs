using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Server.Core.Exceptions
{
    [Serializable]
    public class PoisonedMessageException : Exception
    {
        public PoisonedMessageException()
            : base() { }
        public PoisonedMessageException(string message)
            : base(message) { }
        public PoisonedMessageException(string format, params object[] args)
            : base(string.Format(format, args)) { }
        public PoisonedMessageException(string message, Exception innerException)
            : base(message, innerException) { }
        protected PoisonedMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
