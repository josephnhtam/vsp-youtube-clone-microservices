using Users.Domain.Models;
using Users.Domain.Specifications;

namespace Users.Domain.Contracts {
    public interface IUserChannelRepository {
        Task<UserChannel?> GetUserChannelByIdAsync (string userId, bool includeSections, bool updateLock, CancellationToken cancellationToken = default);
        Task<UserChannel?> GetUserChannelByHandleAsync (string handle, bool includeSections, bool updateLock, CancellationToken cancellationToken = default);
        Task<ChannelSection?> GetChannelSectionAsync (string userId, Guid sectionId, Pagination? itemPagination = null, CancellationToken cancellationToken = default);
        Task<UserChannel?> GetUserChannelByIdAsync (string userId, int? maxSectionItemsCount, CancellationToken cancellationToken = default);
        Task<UserChannel?> GetUserChannelByHandleAsync (string handle, int? maxSectionItemsCount, CancellationToken cancellationToken = default);
        Task AddUserChannelAsync (UserChannel userChannel);
        void Track (UserChannel userChannel);
    }
}
