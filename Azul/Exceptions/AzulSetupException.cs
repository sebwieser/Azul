using System;
using System.Runtime.Serialization;

namespace Azul
{
    [Serializable]
    public class AzulSetupException : Exception
    {
        public AzulSetupException()
        {
        }

        public AzulSetupException(string message) : base(message)
        {
        }

        public AzulSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AzulSetupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}