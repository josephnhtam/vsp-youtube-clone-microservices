using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class RemoveVideoFromPlaylistCommandHandler : ICommandHandler<RemoveVideoFromPlaylistCommand> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveVideoFromPlaylistCommandHandler (IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository, IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (RemoveVideoFromPlaylistCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var playlist = await _playlistRepository.GetPlaylistIncludingVideo(request.PlaylistId, request.VideoId, true, cancellationToken);

                if (playlist == null) {
                    throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
                }

                if (playlist.UserId != request.UserId) {
                    throw new AppException("Only the owner can edit the playlist", null, StatusCodes.Status403Forbidden);
                }

                playlist.RemoveVideo(request.VideoId);
                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
