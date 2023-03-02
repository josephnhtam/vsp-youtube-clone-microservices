namespace History.Domain.Models {
    public class UserVideoHistory {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }
        public UserVideoHistoryType Type { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public int LengthSeconds { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public enum UserVideoHistoryType : int {
        Watch = 0,
        Like = 1,
        Dislike = 2
    }
}
