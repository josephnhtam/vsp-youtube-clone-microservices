using Application.Handlers;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries.Handlers {
    public class GetChannelSectionQueryHandler : IQueryHandler<GetChannelSectionQuery, ChannelSection?> {

        private readonly IUserChannelRepository _repository;

        public GetChannelSectionQueryHandler (IUserChannelRepository repository) {
            _repository = repository;
        }

        public async Task<ChannelSection?> Handle (GetChannelSectionQuery request, CancellationToken cancellationToken) {
            return await _repository.GetChannelSectionAsync(
                request.UserId,
                request.SectionId,
                null,
                cancellationToken);
        }

    }
}
