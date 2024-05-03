using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Exceptions
{
    [Serializable]
    public class BusinessLogicException : Exception
    {
        public readonly ErrorCode Code;

        public BusinessLogicException() { }

        public BusinessLogicException(string message, Exception innerException = null, ErrorCode code = ErrorCode.Other)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}