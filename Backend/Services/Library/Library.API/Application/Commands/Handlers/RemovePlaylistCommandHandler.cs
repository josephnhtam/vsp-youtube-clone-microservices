using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class RemovePlaylistCommandHandler : ICommandHandler<RemovePlaylistCommand> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemovePlaylistCommandHandler (
            IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository,
            IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (RemovePlaylistCommand request, CancellationToken cancellationToken) {
            var playlist = await _playlistRepository.GetPlaylist(request.PlaylistId, false, false, cancellationToken);

            if (playlist == null) {
                throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
            }

            if (playlist.UserId != request.UserId) {
                throw new AppException("Only the owner can remove the playlist", null, StatusCodes.Status403Forbidden);
            }

            await _playlistRepository.RemovePlaylistAsync(request.PlaylistId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
