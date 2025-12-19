using System;

namespace service.Exceptions
{
    public class BoardPurchaseNotAllowedException : Exception
    {
        public BoardPurchaseNotAllowedException() 
            : base("Purchases are not allowed between Saturday 17:00 and Monday 00:00.") { }

        public BoardPurchaseNotAllowedException(string message) : base(message) { }

        public BoardPurchaseNotAllowedException(string message, Exception inner) : base(message, inner) { }
    }
}