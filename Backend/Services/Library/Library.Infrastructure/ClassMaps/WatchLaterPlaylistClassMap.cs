using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class WatchLaterPlaylistClassMap : MongoClassMap<WatchLaterPlaylist> {
        public override void RegisterClassMap (BsonClassMap<WatchLaterPlaylist> classMap) {
            classMap.SetIgnoreExtraElements(true);
        }
    }
}
