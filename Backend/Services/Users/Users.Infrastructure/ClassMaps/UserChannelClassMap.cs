using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class UserChannelClassMap : MongoClassMap<UserChannel> {
        public override void RegisterClassMap (BsonClassMap<UserChannel> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id);

            classMap.MapMember(x => x.Handle).SetIgnoreIfNull(true);

            classMap.MapMember(x => x.Banner);

            classMap.MapMember(x => x.UnsubscribedSpotlightVideoId);

            classMap.MapMember(x => x.SubscribedSpotlightVideoId);

            classMap.MapField("_sections").SetElementName(nameof(UserChannel.Sections));

            classMap.SetIgnoreExtraElements(true);
        }
    }
}

