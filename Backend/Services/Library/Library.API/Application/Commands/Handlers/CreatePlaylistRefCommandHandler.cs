using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class CreatePlaylistRefCommandHandler : ICommandHandler<CreatePlaylistRefCommand> {

        private readonly IPlaylistRefRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePlaylistRefCommandHandler (IPlaylistRefRepository repository, IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (CreatePlaylistRefCommand request, CancellationToken cancellationToken) {
            try {
                var playlistRef = PlaylistRef.Create(request.UserId, request.PlaylistId, DateTimeOffset.UtcNow);
                await _repository.AddPlaylistRefAsync(playlistRef, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            } catch (Exception ex) when (ex.Identify(ExceptionCategories.UniqueViolation)) { }

            return Unit.Value;
        }
    }
}
