using Infrastructure.MongoDb.TransactionalEvents.Models;
using MongoDB.Bson.Serialization;

namespace Infrastructure.MongoDb.TransactionalEvents.ClassMaps {
    public class TransactionalEventsGroupClassMap : MongoClassMap<TransactionalEventsGroup> {

        public override void RegisterClassMap (BsonClassMap<TransactionalEventsGroup> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id);

            classMap.MapMember(x => x.CreateDate);

            classMap.MapMember(x => x.AvailableDate);

            classMap.MapMember(x => x.TransactionalEvents);
        }

    }
}
