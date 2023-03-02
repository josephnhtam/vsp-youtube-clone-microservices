using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VideoManager.SignalRHub.Hubs {
    [Authorize]
    public class VideoManagerHub : Hub<IVideoManagerHubClient> {

    }
}
