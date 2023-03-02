namespace Infrastructure.Caching.Layers {
    public class RedisCacheLayerConfiguration {
        public float ExpirationMultiplier { get; set; } = 1f;
        public TimeSpan MaxExpiration { get; set; } = TimeSpan.MaxValue;
    }
}
