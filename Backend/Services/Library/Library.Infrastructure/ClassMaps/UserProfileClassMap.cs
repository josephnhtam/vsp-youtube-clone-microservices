using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class UserProfileClassMap : MongoClassMap<UserProfile> {
        public override void RegisterClassMap (BsonClassMap<UserProfile> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id);

            classMap.MapMember(x => x.DisplayName);

            classMap.MapMember(x => x.Handle).SetIgnoreIfNull(true);

            classMap.MapMember(x => x.ThumbnailUrl);

            classMap.MapMember(x => x.LastVideoUpdateDate);

            classMap.MapMember(x => x.PrimaryVersion);

            classMap.SetIsRootClass(true);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

