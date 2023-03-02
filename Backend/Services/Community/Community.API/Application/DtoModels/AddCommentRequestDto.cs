namespace Community.API.Application.DtoModels {
    public class AddCommentRequestDto {
        public Guid VideoId { get; set; }
        public string Comment { get; set; }
    }
}
