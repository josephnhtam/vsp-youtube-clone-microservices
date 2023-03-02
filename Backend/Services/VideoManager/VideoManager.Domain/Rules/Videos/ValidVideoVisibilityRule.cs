using Domain.Rules;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.Rules.Videos {
    public class ValidVideoVisibilityRule : DefinedEnumRule<VideoVisibility> {
        public ValidVideoVisibilityRule (VideoVisibility visibility) : base(visibility, "Visibility") { }
    }
}
