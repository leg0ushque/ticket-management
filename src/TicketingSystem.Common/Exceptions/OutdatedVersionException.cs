using System.Runtime.Serialization;

namespace TicketingSystem.Common.Exceptions
{
    public class OutdatedVersionException : Exception
    {
        public OutdatedVersionException()
        {
        }

        public OutdatedVersionException(string message) : base(message)
        {
        }

        public OutdatedVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutdatedVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
