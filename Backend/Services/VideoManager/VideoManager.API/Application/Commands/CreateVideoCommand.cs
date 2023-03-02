using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Commands {
    public class CreateVideoCommand : ICommand<Video> {
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public CreateVideoCommand (string creatorId, string title, string description) {
            CreatorId = creatorId;
            Title = title;
            Description = description;
        }
    }
}
