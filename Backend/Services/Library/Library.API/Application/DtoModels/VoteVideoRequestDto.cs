
using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class VoteVideoRequestDto {
        public Guid VideoId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
