using MongoDB.Bson.Serialization;

namespace Infrastructure.MongoDb {
    public abstract class MongoClassMap<TDocument> {
        public abstract void RegisterClassMap (BsonClassMap<TDocument> classMap);
    }
}
