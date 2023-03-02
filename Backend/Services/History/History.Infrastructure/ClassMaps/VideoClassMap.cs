using History.Domain.Models;
using Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;

namespace History.Infrastructure.ClassMaps {
    public class VideoClassMap : MongoClassMap<Video> {
        public override void RegisterClassMap (BsonClassMap<Video> classMap) {
            classMap.AutoMap();

            classMap.MapIdMember(x => x.Id);

            classMap.MapMember(x => x.CreatorId);

            classMap.MapMember(x => x.Title);

            classMap.MapMember(x => x.Description);

            classMap.MapMember(x => x.Tags);

            classMap.MapMember(x => x.ThumbnailUrl);

            classMap.MapMember(x => x.PreviewThumbnailUrl);

            classMap.MapMember(x => x.LengthSeconds);

            classMap.MapMember(x => x.Visibility);

            classMap.MapMember(x => x.Status);

            classMap.MapMember(x => x.CreateDate);

            classMap.MapMember(x => x.VideoVersion);

            classMap.MapMember(x => x.Metrics);

            classMap.SetIgnoreExtraElements(true);
        }
    }
}
