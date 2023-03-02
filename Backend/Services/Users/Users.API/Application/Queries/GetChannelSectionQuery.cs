using Application.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries {
    public class GetChannelSectionQuery : IQuery<ChannelSection> {
        public string UserId { get; set; }
        public Guid SectionId { get; set; }

        public GetChannelSectionQuery (string userId, Guid sectionId) {
            UserId = userId;
            SectionId = sectionId;
        }
    }
}
