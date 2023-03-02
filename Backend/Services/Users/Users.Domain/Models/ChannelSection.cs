using Domain;

namespace Users.Domain.Models {
    public abstract class ChannelSection : ValueObject {
        public Guid Id { get; private set; }

        public ChannelSection (Guid id) {
            Id = id;
        }
    }

    public abstract class ChannelSection<TItem> : ChannelSection {
        public List<TItem> Items { get; private set; }

        public ChannelSection (Guid id, List<TItem> items) : base(id) {
            Items = items;
        }
    }

    public class VideosSection : ChannelSection<EmptyItem> {
        public VideosSection (Guid id) : base(id, new List<EmptyItem>()) { }
    }

    public class CreatedPlaylistsSection : ChannelSection<EmptyItem> {
        public CreatedPlaylistsSection (Guid id) : base(id, new List<EmptyItem>()) { }
    }

    public class SinglePlaylistSection : ChannelSection<Guid> {
        public SinglePlaylistSection (Guid id, Guid playlistId) :
            base(id, new List<Guid> { playlistId }) {
        }
    }

    public class MultiplePlaylistsSection : ChannelSection<Guid> {
        public string Title { get; private set; }

        public MultiplePlaylistsSection (Guid id, string title, List<Guid> playlistIds) :
            base(id, playlistIds) {
            Title = title;
        }
    }

    public class EmptyItem { }
}
