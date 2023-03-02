using History.Domain.Models;
using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;

namespace History.Infrastructure.ClassMaps {
    public class VideoMetricsClassMap : MongoClassMap<VideoMetrics> {
        public override void RegisterClassMap (BsonClassMap<VideoMetrics> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.ViewsCount);

            classMap.MapMember(x => x.NextSyncDate);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
