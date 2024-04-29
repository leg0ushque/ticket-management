using System;
using System.Runtime.Serialization;

namespace TicketingSystem.BusinessLogic.Exceptions
{
    [Serializable]
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException() { }

        public BusinessLogicException(string message) : base(message)
        {
        }
    }
}