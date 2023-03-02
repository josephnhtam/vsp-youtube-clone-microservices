using Application.Contracts;
using History.API.Application.DtoModels;

namespace History.API.Application.Queries {
    public class GetUserHistorySettingsQuery : IQuery<UserHistorySettingsDto?> {
        public string UserId { get; set; }

        public GetUserHistorySettingsQuery (string userId) {
            UserId = userId;
        }
    }
}
