using ApiGateway.Utilities;
using Yarp.ReverseProxy.Configuration;

namespace ApiGateway.Configurations {
    public partial class Config {

        public static IReadOnlyList<ClusterConfig> GetClusters (IConfiguration configuration) {
            var clusters = new List<ClusterConfig> {
                new ClusterConfig {
                    ClusterId = ClusterId.Community,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:Community")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.History,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:History")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.Library,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:Library")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.Search,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:Search")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.Subscriptions,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:Subscriptions")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.Users,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:Users")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.VideoManager,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:VideoManager")!,
                    })
                },

                new ClusterConfig {
                    ClusterId = ClusterId.VideoManagerSignalR,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:VideoManagerSignalR")!,
                    }),
                    SessionAffinity = new SessionAffinityConfig{
                        Enabled = true,
                        AffinityKeyName = "VideoManager.SignalR"
                    }
                },

                new ClusterConfig {
                    ClusterId = ClusterId.VideoStore,
                    Destinations = CreateSingleDestination(new DestinationConfig {
                        Address = configuration.GetValue<string>("Urls:VideoStore")!,
                    })
                }
            };

            return clusters.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, DestinationConfig> CreateSingleDestination (DestinationConfig config) {
            return new Dictionary<string, DestinationConfig>() { { "Destination", config } };
        }

    }
}
