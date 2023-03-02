using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class PlaylistClassMap : MongoClassMap<Playlist> {
        public override void RegisterClassMap (BsonClassMap<Playlist> classMap) {
            classMap.MapMember(x => x.Title);

            classMap.MapMember(x => x.Description);

            classMap.MapMember(x => x.Visibility);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
