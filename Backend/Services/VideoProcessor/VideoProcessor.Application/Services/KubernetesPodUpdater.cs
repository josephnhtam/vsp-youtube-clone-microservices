using k8s;
using Microsoft.Extensions.Options;
using VideoProcessor.Application.Configurations;

namespace VideoProcessor.Application.Services {
    public class KubernetesPodUpdater : IKubernetesPodUpdater {

        private readonly Kubernetes _kubernetes;
        private readonly KubernetesPodConfiguration _config;
        private readonly ILogger<KubernetesPodUpdater> _logger;

        public KubernetesPodUpdater (Kubernetes kubernetes, IOptions<KubernetesPodConfiguration> config, ILogger<KubernetesPodUpdater> logger) {
            _kubernetes = kubernetes;
            _config = config.Value;
            _logger = logger;
        }

        public async Task SetAnnotationAsync (string key, string value) {
            var podName = _config.PodName;
            var podNamespace = _config.PodNamespace;

            try {
                var pod = await _kubernetes.ReadNamespacedPodAsync(podName, podNamespace);

                if (pod == null) {
                    throw new Exception($"Failed to read namespaced pod for {podName}.{podNamespace}");
                }

                if (pod.Metadata.Annotations == null) pod.Metadata.Annotations = new Dictionary<string, string>();
                pod.Metadata.Annotations[key] = value;

                await _kubernetes.ReplaceNamespacedPodAsync(pod, podName, podNamespace);
            } catch (Exception ex) {
                _logger.LogError(ex, $"Failed to update annotation of {podName}.{podNamespace}");
                throw;
            }
        }

    }
}
