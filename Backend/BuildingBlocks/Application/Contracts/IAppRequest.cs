using MediatR;

namespace Application.Contracts {
    public interface IAppRequest<out TokenResponse> : IRequest<TokenResponse> { }
}
