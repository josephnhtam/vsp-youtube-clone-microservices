using Application.Contracts;
using MediatR;

namespace Application.Handlers {
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult> {
    }
}
