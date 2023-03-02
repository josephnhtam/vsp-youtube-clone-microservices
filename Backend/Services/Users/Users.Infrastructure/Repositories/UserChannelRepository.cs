using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using MongoDB.Driver;
using Users.Domain.Contracts;
using Users.Domain.Models;
using Users.Domain.Specifications;

namespace Users.Infrastructure.Repositories {
    public class UserChannelRepository : IUserChannelRepository {

        private readonly IMongoCollectionContext<UserChannel> _context;
        private readonly IDomainEventEmittersTracker _tracker;

        public UserChannelRepository (
            IMongoCollectionContext<UserChannel> context,
            IDomainEventEmittersTracker tracker) {
            _context = context;
            _tracker = tracker;
        }

        public async Task<UserChannel?> GetUserChannelByIdAsync (string userId, bool includeSections, bool updateLock, CancellationToken cancellationToken = default) {
            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), userId);
            return await GetUserChannelByHandleAsync(filter, includeSections, updateLock, cancellationToken);
        }

        public async Task<UserChannel?> GetUserChannelByHandleAsync (string handle, bool includeSections, bool updateLock, CancellationToken cancellationToken = default) {
            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Handle), handle);
            return await GetUserChannelByHandleAsync(filter, includeSections, updateLock, cancellationToken);
        }

        private async Task<UserChannel?> GetUserChannelByHandleAsync (FilterDefinition<UserChannel> filter, bool includeSections, bool updateLock, CancellationToken cancellationToken = default) {
            var projection = Builders<UserChannel>.Projection
                .Include(nameof(UserChannel.Handle))
                .Include(nameof(UserChannel.Banner));

            if (includeSections) {
                projection = projection.Include(nameof(UserChannel.Sections));
            }

            UserChannel? userChannel;
            if (updateLock) {
                userChannel = await _context.FindOneAndLockAsync(filter, new() { Projection = projection }, cancellationToken);
            } else {
                userChannel = await _context.Collection.Find(filter).Project<UserChannel>(projection).FirstOrDefaultAsync(cancellationToken);
            }

            if (userChannel != null) {
                Track(userChannel);
            }

            return userChannel;
        }

        public async Task<ChannelSection?> GetChannelSectionAsync (string userId, Guid sectionId, Pagination? itemPagination, CancellationToken cancellationToken) {
            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), userId);

            var projection = Builders<UserChannel>.Projection
                .ElemMatch(nameof(UserChannel.Sections),
                    Builders<ChannelSection>.Filter.Eq(nameof(ChannelSection.Id), sectionId));

            if (itemPagination != null) {
                if (itemPagination.PageSize > 0) {
                    projection = Builders<UserChannel>.Projection
                        .Slice($"{nameof(UserChannel.Sections)}.Items",
                        Math.Max(0, (itemPagination.Page - 1) * itemPagination.PageSize), itemPagination.PageSize);
                } else {
                    projection = Builders<UserChannel>.Projection
                        .Exclude($"{nameof(UserChannel.Sections)}.Items");
                }
            }

            var userChannel = await _context.Collection
                .Find(filter)
                .Project<UserChannel>(projection)
                .FirstOrDefaultAsync(cancellationToken);

            if (userChannel != null) {
                Track(userChannel);
            }

            return userChannel?.Sections.FirstOrDefault();
        }

        private async Task<UserChannel?> GetUserChannelByIdAsync (FilterDefinition<UserChannel> filter, int? maxSectionItemsCount, CancellationToken cancellationToken = default) {
            UserChannel? userChannel;

            if (maxSectionItemsCount.HasValue) {
                ProjectionDefinition<UserChannel> projection;

                if (maxSectionItemsCount.Value > 0) {
                    projection = Builders<UserChannel>.Projection
                        .Slice($"{nameof(UserChannel.Sections)}.Items", 0, maxSectionItemsCount.Value);
                } else {
                    projection = Builders<UserChannel>.Projection
                        .Exclude($"{nameof(UserChannel.Sections)}.Items");
                }

                userChannel = await _context.Collection
                    .Find(filter)
                    .Project<UserChannel>(projection)
                    .FirstOrDefaultAsync(cancellationToken);
            } else {
                userChannel = await _context.Collection
                    .Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (userChannel != null) {
                Track(userChannel);
            }

            return userChannel;
        }

        public async Task<UserChannel?> GetUserChannelByIdAsync (string userId, int? maxSectionItemsCount, CancellationToken cancellationToken = default) {
            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), userId);
            return await GetUserChannelByIdAsync(filter, maxSectionItemsCount, cancellationToken);
        }

        public async Task<UserChannel?> GetUserChannelByHandleAsync (string handle, int? maxSectionItemsCount, CancellationToken cancellationToken = default) {
            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Handle), handle);
            return await GetUserChannelByIdAsync(filter, maxSectionItemsCount, cancellationToken);
        }

        public Task AddUserChannelAsync (UserChannel userChannel) {
            Track(userChannel);
            _context.InsertOne(userChannel);
            return Task.CompletedTask;
        }

        public void Track (UserChannel userChannel) {
            _tracker.Track(userChannel);
        }

    }
}
