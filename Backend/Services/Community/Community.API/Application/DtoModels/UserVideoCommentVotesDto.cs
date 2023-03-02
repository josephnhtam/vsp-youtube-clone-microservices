namespace Community.API.Application.DtoModels {
    public class UserVideoCommentVotesDto {
        public IEnumerable<long> LikedCommentIds { get; set; }
        public IEnumerable<long> DislikedCommentIds { get; set; }
    }
}
