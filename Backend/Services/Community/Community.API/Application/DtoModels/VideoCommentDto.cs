namespace Community.API.Application.DtoModels {
    public class VideoCommentDto {
        public long Id { get; set; }
        public UserProfileDto UserProfile { get; private set; }
        public string Comment { get; private set; }
        public int LikesCount { get; private set; }
        public int DislikesCount { get; private set; }
        public int RepliesCount { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? EditDate { get; private set; }
    }
}
