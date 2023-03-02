using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class PlaylistItemClassMap : MongoClassMap<PlaylistItem> {
        public override void RegisterClassMap (BsonClassMap<PlaylistItem> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.Id);

            classMap.MapMember(x => x.VideoId);

            classMap.MapMember(x => x.CreateDate);

            classMap.SetIsRootClass(true);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
