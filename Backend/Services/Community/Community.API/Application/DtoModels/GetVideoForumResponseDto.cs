namespace Community.API.Application.DtoModels {
    public class GetVideoForumResponseDto {
        public int CommentsCount { get; set; }
        public int RootCommentsCount { get; set; }
        public IEnumerable<long> LikedCommentIds { get; set; }
        public IEnumerable<long> DislikedCommentIds { get; set; }
        public List<VideoCommentDto> Comments { get; set; }
        public List<VideoCommentDto>? PinnedUserComments { get; set; }
        public DateTimeOffset LoadTime { get; set; }
    }
}
