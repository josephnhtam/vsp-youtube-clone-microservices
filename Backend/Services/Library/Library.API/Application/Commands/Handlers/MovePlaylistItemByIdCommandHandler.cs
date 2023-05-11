using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class MovePlaylistItemByIdCommandHandler : ICommandHandler<MovePlaylistItemByIdCommand> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MovePlaylistItemByIdCommandHandler (IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository, IUnitOfWork unitOfWork) {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (MovePlaylistItemByIdCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                if (request.PrecedingItemId.HasValue && request.PrecedingItemId.Value == request.ItemId) {
                    throw new AppException(null, null, StatusCodes.Status400BadRequest);
                }

                var playlist = await _playlistRepository.GetPlaylistIncludingItem(request.PlaylistId, request.ItemId, true, cancellationToken);

                if (playlist == null) {
                    throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
                }

                if (!playlist.Items.Any(x => x.Id == request.ItemId)) {
                    throw new AppException("Item not found", null, StatusCodes.Status404NotFound);
                }

                OrderedPlaylistItem? precedingItem = null;

                if (request.PrecedingItemId.HasValue) {
                    precedingItem =
                        (await _playlistRepository.GetPlaylistIncludingItem(request.PlaylistId, request.PrecedingItemId.Value, false, cancellationToken))
                        ?.Items.FirstOrDefault();

                    if (precedingItem == null) {
                        throw new AppException("Item not found", null, StatusCodes.Status404NotFound);
                    }
                }

                var fromPosition = playlist.Items.First(x => x.Id == request.ItemId).Position;

                var toPosition = precedingItem != null ? precedingItem.Position : -1;

                if (fromPosition > toPosition) {
                    toPosition++;
                }

                playlist.MoveItem(fromPosition, toPosition);
                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
