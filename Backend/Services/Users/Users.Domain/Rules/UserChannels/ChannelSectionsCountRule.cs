using Domain.Rules;

namespace Users.Domain.Rules.UserChannels {
    public class ChannelSectionsCountRule : IBusinessRule {

        private readonly int _count;

        public ChannelSectionsCountRule (int count) {
            _count = count;
        }

        public string BrokenReason => "The number of sections is limited by 12";

        public bool IsBroken () {
            return _count > 12;
        }
    }
}
