using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using MongoDB.Driver;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.Infrastructure.Repositories {
    public class UserProfileRepository : IUserProfileRepository {

        private readonly IMongoCollectionContext<UserProfile> _context;
        private readonly IDomainEventEmittersTracker _tracker;

        public UserProfileRepository (
            IMongoCollectionContext<UserProfile> context,
            IDomainEventEmittersTracker tracker) {
            _context = context;
            _tracker = tracker;
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync (string userId, bool updateLock, CancellationToken cancellationToken = default) {
            var filter = Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Id), userId);

            UserProfile? userProfile;
            if (updateLock) {
                userProfile = await _context.FindOneAndLockAsync(filter, null, cancellationToken);
            } else {
                userProfile = await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            }

            if (userProfile != null) {
                Track(userProfile);
            }

            return userProfile;
        }

        public async Task<UserProfile?> GetUserProfileByHandleAsync (string handle, bool updateLock, CancellationToken cancellationToken = default) {
            var filter = Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Handle), handle);

            UserProfile? userProfile;
            if (updateLock) {
                userProfile = await _context.FindOneAndLockAsync(filter, null, cancellationToken);
            } else {
                userProfile = await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            }

            if (userProfile != null) {
                Track(userProfile);
            }

            return userProfile;
        }

        public Task AddUserProfileAsync (UserProfile userProfile) {
            Track(userProfile);
            _context.InsertOne(userProfile);
            return Task.CompletedTask;
        }

        public async Task<bool> IsHandleAvailable (string handle, CancellationToken cancellationToken = default) {
            return (await _context.Collection.CountDocumentsAsync(
                Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Handle), handle),
                null,
                cancellationToken)) == 0;
        }

        public void Track (UserProfile userProfile) {
            _tracker.Track(userProfile);
        }
    }
}
