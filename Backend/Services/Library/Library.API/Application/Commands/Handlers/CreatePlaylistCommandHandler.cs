using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Commands.Handlers {
    public class CreatePlaylistCommandHandler : ICommandHandler<CreatePlaylistCommand, Guid> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePlaylistCommandHandler (
            IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository,
            IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle (CreatePlaylistCommand request, CancellationToken cancellationToken) {
            var playlist = Playlist.Create(request.UserId, request.Title, request.Description, request.Visibility);

            await _playlistRepository.AddPlaylistAsync(playlist, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return playlist.Id;
        }

    }
}
