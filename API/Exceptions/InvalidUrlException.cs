using System;

namespace API.Exceptions {
    public class InvalidUrlException : Exception {
        public InvalidUrlException() {
        }

        public InvalidUrlException(string message) : base(message) {
        }

        public InvalidUrlException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}