using System;

namespace TicketingSystem.BusinessLogic.Exceptions
{
    [Serializable]
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException() { }

        public BusinessLogicException(string message, Exception innerException = null) : base(message, innerException)
        { }
    }
}