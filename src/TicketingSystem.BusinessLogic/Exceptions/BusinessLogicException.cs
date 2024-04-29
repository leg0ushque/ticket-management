using System.Runtime.Serialization;

namespace TicketingSystem.BusinessLogic.Exceptions
{
    [Serializable]
    internal class BusinessLogicException : Exception
    {
        public BusinessLogicException()
        {
        }

        public BusinessLogicException(string message) : base(message)
        {
        }

        public BusinessLogicException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BusinessLogicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}