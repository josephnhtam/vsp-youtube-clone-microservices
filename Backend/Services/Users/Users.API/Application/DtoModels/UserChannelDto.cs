namespace Users.API.Application.DtoModels {
    public class UserChannelDto {
        public string Id { get; set; }
        public string? Handle { get; set; }
        public Guid? UnsubscribedSpotlightVideoId { get; set; }
        public Guid? SubscribedSpotlightVideoId { get; set; }
        public List<ChannelSectionDto> Sections { get; set; }
    }
}
