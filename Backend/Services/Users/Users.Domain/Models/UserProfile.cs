using Domain;
using Users.Domain.DomainEvents;
using Users.Domain.Rules.UserProfiles;

namespace Users.Domain.Models {
    public class UserProfile : DomainEntity, IAggregateRoot {

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string? Handle { get; private set; }
        public string? Email { get; private set; }
        public ImageFile? Thumbnail { get; private set; }
        public UserProfileStatus Status { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }

        public long Version { get; set; }

        private UserProfile () { }

        private UserProfile (string userId, string displayName, ImageFile? thumbnail) {
            CheckRules(new DisplayNameCharactersRule(displayName),
                       new DisplayNameLengthRule(displayName));

            Id = userId;
            DisplayName = displayName;
            Description = string.Empty;
            Handle = null;
            Email = null;
            Status = UserProfileStatus.Created;
            Thumbnail = thumbnail;
            CreateDate = DateTimeOffset.UtcNow;

            AddDomainEvent(new UserProfileCreatedDomainEvent(this));

            if (Thumbnail != null) {
                AddDomainEvent(new UserProfileThumbnailUpdatedDomainEvent(this, null));
            }
        }

        public void UpdateInfo (string displayName, string description, string? handle, string? email) {
            CheckRules(new DisplayNameCharactersRule(displayName),
                       new DisplayNameLengthRule(displayName),
                       new DescriptionLengthRule(description),
                       new HandlePatternRule(handle),
                       new HandleLengthRule(handle),
                       new EmailPatternRule(email),
                       new EmailLengthRule(email));

            DisplayName = displayName;
            Description = description;
            Handle = !string.IsNullOrEmpty(handle) ? handle : null;
            Email = email;
            Version++;

            AddUniqueDomainEvent(new UserProfileInfoUpdatedDomainEvent(this));
            AddUniqueDomainEvent(new UserProfileUpdatedDomainEvent(this));
        }

        public void UpdateThumbnail (ImageFile? thumbnail) {
            if (Thumbnail?.ImageFileId == thumbnail?.ImageFileId) return;

            var oldThumbnail = Thumbnail;
            Thumbnail = thumbnail;
            Version++;

            AddDomainEvent(new UserProfileThumbnailUpdatedDomainEvent(this, oldThumbnail));
            AddUniqueDomainEvent(new UserProfileUpdatedDomainEvent(this));
        }

        public void SetRegisteredStatus () {
            if (Status >= UserProfileStatus.Registered) {
                throw new Exception("Registration already complete or failed");
            }

            Status = UserProfileStatus.Registered;
            Version++;

            AddUniqueDomainEvent(new UserProfileStatusUpdatedDomainEvent(this));
        }

        public void SetRegistrationFailedStatus () {
            if (Status >= UserProfileStatus.Registered) {
                throw new Exception("Registration already complete or failed");
            }

            Status = UserProfileStatus.RegistrationFailed;
            Version++;

            AddUniqueDomainEvent(new UserProfileStatusUpdatedDomainEvent(this));
        }

        public void IncrementVersion () {
            Version++;
        }

        public static UserProfile Create (string userId, string displayName, ImageFile? thumbnail) {
            return new UserProfile(userId, displayName, thumbnail);
        }

    }
}
