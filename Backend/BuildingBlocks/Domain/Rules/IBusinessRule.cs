namespace Domain.Rules {
    public interface IBusinessRule {
        bool IsBroken ();
        string BrokenReason { get; }
    }
}
