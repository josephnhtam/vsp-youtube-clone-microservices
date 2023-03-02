using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class ImageFileClassMap : MongoClassMap<ImageFile> {
        public override void RegisterClassMap (BsonClassMap<ImageFile> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.ImageFileId);

            classMap.MapMember(x => x.Url);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

