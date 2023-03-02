using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class CreatedPlaylistsSectionClassMap : MongoClassMap<CreatedPlaylistsSection> {
        public override void RegisterClassMap (BsonClassMap<CreatedPlaylistsSection> classMap) {
            classMap.AutoMap();

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

