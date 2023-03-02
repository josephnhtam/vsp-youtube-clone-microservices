using Infrastructure.MongoDb;
using Library.Domain.Models;
using MongoDB.Bson.Serialization;

namespace Library.Infrastructure.ClassMaps {
    public class VideoMetricsClassMap : MongoClassMap<VideoMetrics> {
        public override void RegisterClassMap (BsonClassMap<VideoMetrics> classMap) {
            classMap.AutoMap();

            classMap.MapMember(x => x.ViewsCount);

            classMap.MapMember(x => x.LikesCount);

            classMap.MapMember(x => x.DislikesCount);

            classMap.MapMember(x => x.ViewsCountUpdateDate);

            classMap.MapMember(x => x.NextSyncDate);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
