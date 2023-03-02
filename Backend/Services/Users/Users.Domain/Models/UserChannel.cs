using Domain;
using Users.Domain.DomainEvents;
using Users.Domain.Rules.UserChannels;
using Users.Domain.Rules.UserProfiles;

namespace Users.Domain.Models {
    public class UserChannel : DomainEntity, IAggregateRoot {

        private List<ChannelSection> _sections;

        public string Id { get; private set; }
        public string? Handle { get; private set; }
        public ImageFile? Banner { get; private set; }
        public Guid? UnsubscribedSpotlightVideoId { get; private set; }
        public Guid? SubscribedSpotlightVideoId { get; private set; }
        public IReadOnlyList<ChannelSection> Sections => _sections.AsReadOnly();

        private UserChannel () {
            _sections = new List<ChannelSection>();
        }

        private UserChannel (string id, string? handle) : this() {
            CheckRules(new HandlePatternRule(handle), new HandleLengthRule(handle));

            Id = id;
            Handle = !string.IsNullOrEmpty(handle) ? handle : null;

            _sections.Add(new VideosSection(Guid.NewGuid()));
            _sections.Add(new CreatedPlaylistsSection(Guid.NewGuid()));
        }

        public static UserChannel Create (string id, string? handle) {
            return new UserChannel(id, handle);
        }

        public void UpdateBanner (ImageFile? banner) {
            if (Banner?.ImageFileId == banner?.ImageFileId) return;

            var oldBanner = Banner;
            Banner = banner;

            AddDomainEvent(new UserChannelBannerUpdatedDomainEvent(this, oldBanner));
        }

        public void UpdateChannelLayout (
            Guid? unsubscribedSpotlightVideoId,
            Guid? subscribedSpotlightVideoId,
            List<ChannelSection> sections) {
            CheckRule(new ChannelSectionsCountRule(sections.Count));

            UnsubscribedSpotlightVideoId = unsubscribedSpotlightVideoId;
            SubscribedSpotlightVideoId = subscribedSpotlightVideoId;

            _sections.Clear();
            _sections.AddRange(sections);

            AddDomainEvent(new UserChannelLayoutUpdatedDomainEvent(this));
        }

    }
}
