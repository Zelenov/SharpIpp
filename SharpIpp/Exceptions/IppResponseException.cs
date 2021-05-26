using System;
using System.Runtime.Serialization;
using SharpIpp.Model;

namespace SharpIpp.Exceptions
{
    [Serializable]
    public class IppResponseException : Exception
    {
        public IppResponseException(IppResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        protected IppResponseException(SerializationInfo info, StreamingContext context,
            IppResponseMessage responseMessage) : base(info, context)
        {
            ResponseMessage = responseMessage;
        }

        public IppResponseException(string message, IppResponseMessage responseMessage) : base(message)
        {
            ResponseMessage = responseMessage;
        }

        public IppResponseException(string message, Exception innerException, IppResponseMessage responseMessage) :
            base(message, innerException)
        {
            ResponseMessage = responseMessage;
        }

        public IppResponseMessage ResponseMessage { get; set; }

        public override string ToString() => $"{base.ToString()}\n{nameof(ResponseMessage)}: {ResponseMessage}";
    }
}