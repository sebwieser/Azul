using System;
using System.Runtime.Serialization;

namespace Azul {

  [Serializable]
  public class AzulGameplayException : Exception {

    public AzulGameplayException() {
    }

    public AzulGameplayException(string message) : base(message) {
    }

    public AzulGameplayException(string message, Exception innerException) : base(message, innerException) {
    }

    protected AzulGameplayException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
  }
}