namespace Search.Infrastructure.Specifications {
    public class PeriodRange {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }

        public PeriodRange (DateTimeOffset? from, DateTimeOffset? to) {
            From = from;
            To = to;
        }
    }
}
