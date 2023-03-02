using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Library.Infrastructure.ClassMaps {
    public class PlaylistBaseClassMap : MongoClassMap<PlaylistBase> {
        public override void RegisterClassMap (BsonClassMap<PlaylistBase> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id).SetIdGenerator(CombGuidGenerator.Instance);

            classMap.MapMember(x => x.UserId);

            classMap.MapMember(x => x.ItemsCount);

            classMap.MapMember(x => x.CreateDate);

            classMap.MapMember(x => x.UpdateDate);

            classMap.SetIsRootClass(true);

            classMap.SetIgnoreExtraElements(true);
        }
    }

    public class PlaylistBasePlaylistItemClassMap : MongoClassMap<PlaylistBase<PlaylistItem>> {
        public override void RegisterClassMap (BsonClassMap<PlaylistBase<PlaylistItem>> classMap) {
            classMap.AutoMap();

            classMap.MapField("_items").SetElementName(nameof(PlaylistBase<PlaylistItem>.Items));

            classMap.SetIgnoreExtraElements(true);
        }
    }

    public class PlaylistBaseOrderedPlaylistItemClassMap : MongoClassMap<PlaylistBase<OrderedPlaylistItem>> {
        public override void RegisterClassMap (BsonClassMap<PlaylistBase<OrderedPlaylistItem>> classMap) {
            classMap.AutoMap();

            classMap.MapField("_items").SetElementName(nameof(PlaylistBase<OrderedPlaylistItem>.Items));

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
