using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using Users.Domain.Models;

namespace Users.Infrastructure.ClassMaps {
    public class ChannelSectionClassMap : MongoClassMap<ChannelSection> {
        public override void RegisterClassMap (BsonClassMap<ChannelSection> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.Id);

            classMap.SetIsRootClass(true);

            classMap.SetIgnoreExtraElements(true);
        }
    }

    public class ChannelSectionEmptyItemClassMap : MongoClassMap<ChannelSection<EmptyItem>> {
        public override void RegisterClassMap (BsonClassMap<ChannelSection<EmptyItem>> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.Items);
        }
    }

    public class ChannelSectionGuidItemClassMap : MongoClassMap<ChannelSection<Guid>> {
        public override void RegisterClassMap (BsonClassMap<ChannelSection<Guid>> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.Items);
        }
    }
}

