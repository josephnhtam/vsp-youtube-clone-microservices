using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class MultiplePlaylistsSectionClassMap : MongoClassMap<MultiplePlaylistsSection> {
        public override void RegisterClassMap (BsonClassMap<MultiplePlaylistsSection> classMap) {
            classMap.AutoMap();

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

