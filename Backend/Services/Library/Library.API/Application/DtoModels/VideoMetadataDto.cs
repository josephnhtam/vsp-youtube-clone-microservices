using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class VideoMetadataDto {
        public long ViewsCount { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public VoteType UserVote { get; set; }
    }
}
