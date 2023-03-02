using Infrastructure.Idempotency;
using MongoDB.Bson.Serialization;

namespace Infrastructure.MongoDb.Idempotency.ClassMaps {
    public class IdempotentOperationClassMap : MongoClassMap<IdempotentOperation> {

        public override void RegisterClassMap (BsonClassMap<IdempotentOperation> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id);

            classMap.MapMember(x => x.Date);
        }

    }
}
