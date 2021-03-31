using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace API.Exceptions {
    public class DataContextException : DbException {
        public DataContextException() {
        }

        public DataContextException(string message) : base(message) {
        }

        public DataContextException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        public DataContextException(string message, Exception innerException) : base(message, innerException) {
        }

        public DataContextException(string message, int errorCode) : base(message, errorCode) {
        }
    }
}