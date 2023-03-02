using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Library.Infrastructure.ClassMaps {
    public class PlaylistRefClassMap : MongoClassMap<PlaylistRef> {
        public override void RegisterClassMap (BsonClassMap<PlaylistRef> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id).SetIdGenerator(CombGuidGenerator.Instance);

            classMap.MapMember(x => x.UserId);

            classMap.MapMember(x => x.PlaylistId);

            classMap.MapMember(x => x.CreateDate);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
