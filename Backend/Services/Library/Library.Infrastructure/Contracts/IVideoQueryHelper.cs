using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Infrastructure.Contracts {
    public interface IVideoQueryHelper {
        Task<GetVideosResult> GetVideosAsync (string userId, bool publicOnly, VideoSort videoSort, int? itemsSkip, int? itemsLimit, CancellationToken cancellationToken = default);
        Task<int> GetVideoIndex (string userId, bool publicOnly, Guid videoId, VideoSort videoSort, CancellationToken cancellationToken = default);
        Task<long> GetVideoTotalViews (string userId, bool publicOnly, CancellationToken cancellationToken = default);
    }

    public class GetVideosResult {
        public int TotalCount { get; set; }
        public List<Video> Videos { get; set; }
    }
}
