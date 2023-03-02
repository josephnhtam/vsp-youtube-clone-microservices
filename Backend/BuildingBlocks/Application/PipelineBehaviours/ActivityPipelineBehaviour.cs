using Application.Contracts;
using MediatR;
using SharedKernel.Utilities;
using System.Diagnostics;

namespace Application.PipelineBehaviours {
    public static class ActivityPipelineBehaviour {
        public static readonly ActivitySource ActivitySource = new ActivitySource("Mediator");
    }

    public class ActivityPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IAppRequest<TResponse> {

        private static readonly string _requestName;

        static ActivityPipelineBehaviour () {
            _requestName = typeof(TRequest).GetGenericTypeName();
        }

        public async Task<TResponse> Handle (TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            using var activity = ActivityPipelineBehaviour.ActivitySource.StartActivity(_requestName);
            return await next();
        }
    }

}
