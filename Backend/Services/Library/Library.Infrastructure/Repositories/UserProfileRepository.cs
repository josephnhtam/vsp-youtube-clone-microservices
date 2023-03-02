using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.Repositories {
    public class UserProfileRepository : IUserProfileRepository {

        private readonly IMongoCollectionContext<UserProfile> _context;
        private readonly IDomainEventEmittersTracker _emittersTracker;

        public UserProfileRepository (IMongoCollectionContext<UserProfile> context, IDomainEventEmittersTracker emittersTracker) {
            _context = context;
            _emittersTracker = emittersTracker;
        }

        public Task AddUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            _emittersTracker.Track(userProfile);
            _context.InsertOne(userProfile);
            return Task.CompletedTask;
        }

        public Task DeleteUserProfileAsync (string userId, CancellationToken cancellationToken = default) {
            _context.DeleteOne(Builders<UserProfile>.Filter.Eq(x => x.Id, userId));
            return Task.CompletedTask;
        }

        public async Task<UserProfile?> GetUserProfileAsync (string userId, bool updateLock = false, CancellationToken cancellationToken = default) {
            UserProfile? userProfile;

            var filter = Builders<UserProfile>.Filter.Eq(x => x.Id, userId);

            if (updateLock) {
                userProfile = await _context.FindOneAndLockAsync(filter, null, cancellationToken);
            } else {
                userProfile = await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            }

            if (userProfile != null) {
                _emittersTracker.Track(userProfile);
            }

            return userProfile;
        }

        public async Task<List<UserProfile>> GetUserProfilesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default) {
            var queryTasks = userIds.Chunk(256).Select(idsChunk => {
                return _context.Collection.Find(Builders<UserProfile>.Filter.In(x => x.Id, idsChunk)).ToListAsync(cancellationToken);
            }).ToList();

            await Task.WhenAll(queryTasks);

            var result = queryTasks.SelectMany(x => x.Result).ToList();

            foreach (var userProfile in result) {
                _emittersTracker.Track(userProfile);
            }

            return result;
        }

        public void Track (UserProfile userProfile) {
            _emittersTracker.Track(userProfile);
        }
    }
}
