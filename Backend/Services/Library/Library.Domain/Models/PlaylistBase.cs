using Domain;
using Library.Domain.DomainEvents.Playlists;

namespace Library.Domain.Models {
    public abstract class PlaylistBase : DomainEntity, IAggregateRoot {

        public Guid Id { get; private set; }
        public string UserId { get; private set; }

        public int ItemsCount { get; protected set; }
        public DateTimeOffset CreateDate { get; protected set; }
        public DateTimeOffset UpdateDate { get; protected set; }

        protected PlaylistBase () {
        }

        protected PlaylistBase (Guid id, string userId) {
            Id = id;
            UserId = userId;
            CreateDate = DateTimeOffset.Now;
            UpdateDate = DateTimeOffset.Now;
        }

    }

    public abstract class PlaylistBase<TItem> : PlaylistBase where TItem : PlaylistItem {

        protected List<TItem> _items;
        public IReadOnlyList<TItem> Items => _items.AsReadOnly();

        protected PlaylistBase () {
            _items = new List<TItem>();
        }

        protected PlaylistBase (Guid id, string userId) : base(id, userId) {
            _items = new List<TItem>();
        }

    }

    public abstract class UnorderedPlaylistBase<T> : PlaylistBase<PlaylistItem> where T : UnorderedPlaylistBase<T> {

        protected UnorderedPlaylistBase () {
        }

        protected UnorderedPlaylistBase (Guid id, string userId) : base(id, userId) {
        }

        public bool AddVideo (Guid videoId, bool avoidDuplicate) {
            if (!avoidDuplicate || !_items.Any(x => x.VideoId == videoId)) {
                var itemId = Guid.NewGuid();
                _items.Add(new PlaylistItem(itemId, videoId, DateTimeOffset.UtcNow));
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new VideoAddedToUnorderedPlaylistDomainEvent<T>(Id, itemId, videoId, UpdateDate));
                return true;
            }
            return false;
        }

        public bool RemoveVideo (Guid videoId) {
            var item = _items.FirstOrDefault(x => x.VideoId == videoId);

            if (item != null) {
                _items.Remove(item);
                ItemsCount--;
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new ItemRemovedFromUnorderedPlaylistDomainEvent<T>(Id, item.Id, UpdateDate));
                return true;
            }
            return false;
        }

        public bool RemoveItem (Guid itemId) {
            var item = _items.FirstOrDefault(x => x.Id == itemId);

            if (item != null) {
                _items.Remove(item);
                ItemsCount--;
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new ItemRemovedFromUnorderedPlaylistDomainEvent<T>(Id, itemId, UpdateDate));
                return true;
            }
            return false;
        }

    }

    public abstract class OrderedPlaylistBase<T> : PlaylistBase<OrderedPlaylistItem> where T : OrderedPlaylistBase<T> {

        protected OrderedPlaylistBase () { }

        protected OrderedPlaylistBase (Guid id, string userId) : base(id, userId) { }

        public virtual bool AddVideo (Guid videoId, bool avoidDuplicate) {
            if (!avoidDuplicate || !_items.Any(x => x.VideoId == videoId)) {
                var position = ItemsCount++;

                var itemId = Guid.NewGuid();
                _items.Add(new OrderedPlaylistItem(itemId, videoId, position, DateTimeOffset.UtcNow));
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new VideoAddedToOrderedPlaylistDomainEvent<T>(Id, itemId, videoId, position, UpdateDate));
                return true;
            }
            return false;
        }

        public virtual bool RemoveVideo (Guid videoId) {
            var item = _items.FirstOrDefault(x => x.VideoId == videoId);

            if (item != null) {
                _items.Remove(item);
                ItemsCount--;
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new ItemRemovedFromOrderedPlaylistDomainEvent<T>(Id, item.Id, item.Position, UpdateDate));
                return true;
            }
            return false;
        }

        public virtual bool RemoveItem (Guid itemId) {
            var item = _items.FirstOrDefault(x => x.Id == itemId);

            if (item != null) {
                _items.Remove(item);
                ItemsCount--;
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new ItemRemovedFromOrderedPlaylistDomainEvent<T>(Id, itemId, item.Position, UpdateDate));
                return true;
            }
            return false;
        }

        public virtual void MoveItem (int fromPosition, int toPosition) {
            fromPosition = Math.Clamp(fromPosition, 0, ItemsCount - 1);
            toPosition = Math.Clamp(toPosition, 0, ItemsCount - 1);

            UpdateDate = DateTimeOffset.UtcNow;

            if (fromPosition < toPosition) {
                var targetItem = _items.FirstOrDefault(x => x.Position == fromPosition);

                _items.ForEach(x => {
                    if (x.Position > fromPosition && x.Position <= toPosition) {
                        x.SetPosition(x.Position - 1);
                    }
                });

                if (targetItem != null) {
                    targetItem.SetPosition(toPosition);
                }

                AddDomainEvent(new PlaylistItemMovedDomainEvent<T>(Id, fromPosition, toPosition, UpdateDate));
            } else if (fromPosition > toPosition) {
                var targetItem = _items.FirstOrDefault(x => x.Position == fromPosition);

                _items.ForEach(x => {
                    if (x.Position < fromPosition && x.Position >= toPosition) {
                        x.SetPosition(x.Position + 1);
                    }
                });

                if (targetItem != null) {
                    targetItem.SetPosition(toPosition);
                }

                AddDomainEvent(new PlaylistItemMovedDomainEvent<T>(Id, fromPosition, toPosition, UpdateDate));
            }
        }

    }

}

