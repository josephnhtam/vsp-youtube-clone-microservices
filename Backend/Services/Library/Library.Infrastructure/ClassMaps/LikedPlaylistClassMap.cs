using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class LikedPlaylistClassMap : MongoClassMap<LikedPlaylist> {
        public override void RegisterClassMap (BsonClassMap<LikedPlaylist> classMap) {
            classMap.SetIgnoreExtraElements(true);
        }
    }
}
