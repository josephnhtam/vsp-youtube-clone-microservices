using MediatR;

namespace Application.Contracts {
    public interface ICommand : ICommand<Unit> {
    }

    public interface ICommand<out TResult> : IAppRequest<TResult> {
    }
}
