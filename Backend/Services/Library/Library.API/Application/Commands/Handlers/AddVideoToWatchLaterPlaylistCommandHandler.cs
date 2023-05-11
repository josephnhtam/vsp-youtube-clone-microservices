using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class AddVideoToWatchLaterPlaylistCommandHandler : ICommandHandler<AddVideoToWatchLaterPlaylistCommand> {

        private readonly IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddVideoToWatchLaterPlaylistCommandHandler (
            IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> playlistRepository,
            IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (AddVideoToWatchLaterPlaylistCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var playlist = await _playlistRepository.GetPlaylist(request.UserId, false, true, cancellationToken);

                if (playlist == null) {
                    throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
                }

                playlist.AddVideo(request.VideoId, false);
                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
