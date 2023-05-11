using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class UpdatePlaylistCommandHandler : ICommandHandler<UpdatePlaylistCommand> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePlaylistCommandHandler (IPlaylistRepository<Playlist, OrderedPlaylistItem> repository, IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (UpdatePlaylistCommand request, CancellationToken cancellationToken) {
            var playlist = await _repository.GetPlaylist(request.PlaylistId, false, false, cancellationToken);

            if (playlist == null) {
                throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
            }

            if (playlist.UserId != request.UserId) {
                throw new AppException("Only the owner can edit the playlist", null, StatusCodes.Status403Forbidden);
            }

            playlist.Update(request.Title, request.Description, request.Visibility);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Unit.Value;
        }

    }
}
