using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class VideosSectionClassMap : MongoClassMap<VideosSection> {
        public override void RegisterClassMap (BsonClassMap<VideosSection> classMap) {
            classMap.AutoMap();

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

