using System;
using System.Runtime.Serialization;
using SharpIpp.Model;

namespace SharpIpp.Exceptions
{
    [Serializable]
    public class IppResponseException : Exception
    {
        public IppResponseException(IppResponse response)
        {
            Response = response;
        }

        protected IppResponseException(SerializationInfo info, StreamingContext context, IppResponse response) : base(
            info, context)
        {
            Response = response;
        }

        public IppResponseException(string message, IppResponse response) : base(message)
        {
            Response = response;
        }

        public IppResponseException(string message, Exception innerException, IppResponse response) : base(message,
            innerException)
        {
            Response = response;
        }

        public IppResponse Response { get; set; }

        public override string ToString() => $"{base.ToString()}\n{nameof(Response)}: {Response}";
    }
}