using System;
using System.Runtime.Serialization;

namespace Azul {

  [Serializable]
  public class AzulPlacementException : Exception {
    public AzulPlacementException() {
    }

    public AzulPlacementException(string message) : base(message) {
    }

    public AzulPlacementException(string message, Exception innerException) : base(message, innerException) {
    }

    protected AzulPlacementException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
  }
}