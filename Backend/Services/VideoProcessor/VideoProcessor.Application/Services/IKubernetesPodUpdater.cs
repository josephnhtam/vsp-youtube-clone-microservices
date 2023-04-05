namespace VideoProcessor.Application.Services {
    public interface IKubernetesPodUpdater {
        Task SetAnnotationAsync (string key, string value);
    }
}
