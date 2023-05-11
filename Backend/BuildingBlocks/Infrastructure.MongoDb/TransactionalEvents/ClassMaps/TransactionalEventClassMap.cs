using Domain.TransactionalEvents;
using MongoDB.Bson.Serialization;

namespace Infrastructure.MongoDb.TransactionalEvents.ClassMaps {
    public class TransactionalEventClassMap : MongoClassMap<TransactionalEvent> {

        public override void RegisterClassMap (BsonClassMap<TransactionalEvent> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.Category).SetIsRequired(true);

            classMap.MapMember(x => x.Type).SetIsRequired(true);

            classMap.MapMember(x => x.Data).SetIsRequired(true);
        }

    }
}
