namespace Users.API.Application.DtoModels {
    public class UpdateUserRequestDto {
        public UpdateUserProfileBasicInfo? UpdateBasicInfo { get; set; }
        public UpdateUserBrandingImages? UpdateImages { get; set; }
        public UpdateUserChannelLayout? UpdateLayout { get; set; }
    }

    public class UpdateUserProfileBasicInfo {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string? Handle { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateUserBrandingImages {
        public bool ThumbnailChanged { get; set; }
        public bool BannerChanged { get; set; }
        public string? NewThubmnailToken { get; set; }
        public string? NewBannerToken { get; set; }
    }

    public class UpdateUserChannelLayout {
        public Guid? UnsubscribedSpotlightVideoId { get; set; }
        public Guid? SubscribedSpotlightVideoId { get; set; }
        public List<ChannelSectionDto> ChannelSections { get; set; }
    }
}
