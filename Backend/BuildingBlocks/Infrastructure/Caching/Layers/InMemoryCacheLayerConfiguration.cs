namespace Infrastructure.Caching.Layers {
    public class InMemoryCacheLayerConfiguration {
        public float ExpirationMultiplier { get; set; } = 1f;
        public TimeSpan MaxExpiration { get; set; } = TimeSpan.FromHours(1);
    }
}
