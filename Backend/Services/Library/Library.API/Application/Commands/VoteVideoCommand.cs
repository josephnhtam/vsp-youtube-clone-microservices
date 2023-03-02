using Application.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Commands {
    public class VoteVideoCommand : ICommand {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }
        public VoteType VoteType { get; set; }

        public VoteVideoCommand (string userId, Guid videoId, VoteType voteType) {
            UserId = userId;
            VideoId = videoId;
            VoteType = voteType;
        }
    }
}
