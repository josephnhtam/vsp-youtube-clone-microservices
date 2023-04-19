namespace SharedKernel.Exceptions {
    public static class ExceptionExtensions {
        public static TException? FindInnerException<TException> (this Exception ex) where TException : Exception {
            if (ex is TException exception) {
                return exception;
            }

            return ex.InnerException?.FindInnerException<TException>();
        }

        public static Exception? FindInnerException (this Exception ex, Type exceptionType) {
            if (exceptionType.IsAssignableFrom(ex.GetType())) {
                return ex;
            }

            return ex.InnerException?.FindInnerException(exceptionType);
        }
    }
}
