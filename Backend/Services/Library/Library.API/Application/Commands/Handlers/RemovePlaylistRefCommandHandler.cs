using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using MediatR;

namespace Library.API.Application.Commands.Handlers {
    public class RemovePlaylistRefCommandHandler : ICommandHandler<RemovePlaylistRefCommand> {

        private readonly IPlaylistRefRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RemovePlaylistRefCommandHandler (IPlaylistRefRepository repository, IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (RemovePlaylistRefCommand request, CancellationToken cancellationToken) {
            await _repository.RemovePlaylistRefAsync(request.UserId, request.PlaylistId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
