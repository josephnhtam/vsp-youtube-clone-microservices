using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class RemoveVideoFromWatchLaterPlaylistCommandHandler : ICommandHandler<RemoveVideoFromWatchLaterPlaylistCommand> {

        private readonly IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveVideoFromWatchLaterPlaylistCommandHandler (
            IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> playlistRepository,
            IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (RemoveVideoFromWatchLaterPlaylistCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var playlist = await _playlistRepository.GetPlaylistIncludingVideo(request.UserId, request.VideoId, true, cancellationToken);

                if (playlist == null) {
                    throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
                }

                playlist.RemoveVideo(request.VideoId);
                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
