using System;
using System.Runtime.Serialization;

namespace Azul {

  [Serializable]
  public class NotOnTurnException : Exception {

    public NotOnTurnException() {
    }

    public NotOnTurnException(string message) : base(message) {
    }

    public NotOnTurnException(string message, Exception innerException) : base(message, innerException) {
    }

    protected NotOnTurnException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
  }
}