using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class DislikedPlaylistClassMap : MongoClassMap<DislikedPlaylist> {
        public override void RegisterClassMap (BsonClassMap<DislikedPlaylist> classMap) {
            classMap.SetIgnoreExtraElements(true);
        }
    }
}
