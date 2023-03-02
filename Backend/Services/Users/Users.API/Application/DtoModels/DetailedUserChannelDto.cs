namespace Users.API.Application.DtoModels {
    public class DetailedUserChannelDto {
        public string? BannerUrl { get; set; }
        public Guid? UnsubscribedSpotlightVideoId { get; set; }
        public Guid? SubscribedSpotlightVideoId { get; set; }
        public List<ChannelSectionDto> Sections { get; set; }
    }
}
