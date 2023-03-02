namespace History.API.Application.DtoModels {
    public class UserWatchRecordDto {
        public string Id { get; set; }
        public object Video { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
