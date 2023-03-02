using ApiGateway.Utilities;
using Yarp.ReverseProxy.Configuration;

namespace ApiGateway.Configurations {
    public partial class Config {

        public static IReadOnlyList<RouteConfig> GetRoutes (IConfiguration configuration) {
            var routes = new List<RouteConfig>() {
                new RouteConfig {
                    RouteId = RouteId.VideoForum,
                    ClusterId = ClusterId.Community,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/VideoForum/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.UserHistory,
                    ClusterId = ClusterId.History,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/UserHistory/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.Library,
                    ClusterId = ClusterId.Library,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/Library/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.PlaylistLibrary,
                    ClusterId = ClusterId.Library,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/PlaylistLibrary/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.VideoLibrary,
                    ClusterId = ClusterId.Library,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/VideoLibrary/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.Search,
                    ClusterId = ClusterId.Search,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/Search/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.Subscriptions,
                    ClusterId = ClusterId.Subscriptions,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/Subscriptions/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.Notifications,
                    ClusterId = ClusterId.Subscriptions,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/Notifications/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.UserChannels,
                    ClusterId = ClusterId.Users,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/UserChannels/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.UserProfiles,
                    ClusterId = ClusterId.Users,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/UserProfiles/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.Users,
                    ClusterId = ClusterId.Users,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/Users/{**catch-all}",
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.VideoManager,
                    ClusterId = ClusterId.VideoManager,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/VideoManager/{**catch-all}"
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.VideoManagerSignalR,
                    ClusterId = ClusterId.VideoManagerSignalR,
                    CorsPolicy = "HubCorsPolicy",
                    Match = new RouteMatch{
                        Path = "/Hubs/VideoManager/{**catch-all}"
                    }
                },

                new RouteConfig {
                    RouteId = RouteId.VideoStore,
                    ClusterId = ClusterId.VideoStore,
                    CorsPolicy = "CorsPolicy",
                    Match = new RouteMatch{
                        Path = "api/v1/VideoStore/{**catch-all}",
                    }
                }
            };

            return routes.AsReadOnly();
        }

    }
}
