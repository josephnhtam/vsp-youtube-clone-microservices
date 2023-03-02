using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class OrderedPlaylistItemClassMap : MongoClassMap<OrderedPlaylistItem> {
        public override void RegisterClassMap (BsonClassMap<OrderedPlaylistItem> classMap) {
            classMap.MapMember(x => x.Position);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
