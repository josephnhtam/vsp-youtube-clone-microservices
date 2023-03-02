using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class SinglePlaylistSectionClassMap : MongoClassMap<SinglePlaylistSection> {
        public override void RegisterClassMap (BsonClassMap<SinglePlaylistSection> classMap) {
            classMap.AutoMap();

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

