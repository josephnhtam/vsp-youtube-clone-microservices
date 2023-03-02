namespace Application.Contracts {
    public interface IQuery<out TResult> : IAppRequest<TResult> {
    }
}
