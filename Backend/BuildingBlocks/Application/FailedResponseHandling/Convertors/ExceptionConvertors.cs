using Application.DtoModels;
using Microsoft.AspNetCore.Http;

namespace Application.FailedResponseHandling.Convertors {
    public static class ExceptionConvertors {

        private static List<IExceptionConvertor> _convertors;

        static ExceptionConvertors () {
            Register(null);
        }

        public static void Register (Action<IExceptionConvertorsBuilder>? configure) {
            var builder = new ExceptionConvertorsBuilder();
            configure?.Invoke(builder);
            AddDefaultConvertors(builder);
            _convertors = builder.Build();
        }

        private static void AddDefaultConvertors (IExceptionConvertorsBuilder builder) {
            builder.AddConvertor(new BusinessRuleValidationExceptionConvertor());
            builder.AddConvertor(new ValidationExceptionConvertor());
            builder.AddConvertor(new AppExceptionConvertor());
            builder.AddConvertor(new HttpRequestExceptionConvertor());
        }

        public static FailedResponseDTO ToFailedResponse (this Exception exception) {
            var convertors = _convertors;

            foreach (var convertor in convertors) {
                var result = convertor.ToFailedResponse(exception);
                if (result != null) {
                    return result;
                }
            }

            if (exception.InnerException != null) {
                return exception.InnerException.ToFailedResponse();
            } else {
                return new FailedResponseDTO(StatusCodes.Status500InternalServerError, string.Empty);
            }
        }
    }

    public interface IExceptionConvertorsBuilder {
        void AddConvertor (IExceptionConvertor identifier);
        void Clear ();
    }

    internal class ExceptionConvertorsBuilder : IExceptionConvertorsBuilder {

        private List<IExceptionConvertor> _convertors;

        public ExceptionConvertorsBuilder () {
            _convertors = new List<IExceptionConvertor>();
        }

        public void Clear () {
            _convertors.Clear();
        }

        public void AddConvertor (IExceptionConvertor identifier) {
            _convertors.Add(identifier);
        }

        public List<IExceptionConvertor> Build () {
            return _convertors;
        }

    }
}
