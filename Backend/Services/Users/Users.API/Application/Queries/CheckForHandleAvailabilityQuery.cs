using Application.Contracts;

namespace Users.API.Application.Queries {
    public class CheckForHandleAvailabilityQuery : IQuery<bool> {
        public string Handle { get; set; }

        public CheckForHandleAvailabilityQuery (string handle) {
            Handle = handle;
        }
    }
}
