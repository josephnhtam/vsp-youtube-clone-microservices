using Moq;
using System.Collections;
using VideoManager.API.Commands;
using VideoManager.Domain.Rules.Videos;

namespace VideoManager.UnitTests.Data {
    public class CreateVideoCommandData : IEnumerable<object[]> {

        public IEnumerator<object[]> GetEnumerator () {
            string validTile = Utilities.GenerateString(TitleLengthRule.MaxLength);
            string validDescription = Utilities.GenerateString(DescriptionLengthRule.MaxLength);

            string tooLongTitle = Utilities.GenerateString(TitleLengthRule.MaxLength + 1);
            string tooLongDescription = Utilities.GenerateString(DescriptionLengthRule.MaxLength + 1);

            yield return new object[] { false, new CreateVideoCommand(It.IsAny<string>(), string.Empty, string.Empty) };
            yield return new object[] { false, new CreateVideoCommand(It.IsAny<string>(), string.Empty, validDescription) };

            yield return new object[] { true, new CreateVideoCommand(It.IsAny<string>(), validTile, string.Empty) };
            yield return new object[] { true, new CreateVideoCommand(It.IsAny<string>(), validTile, validDescription) };

            yield return new object[] { false, new CreateVideoCommand(It.IsAny<string>(), tooLongTitle, validDescription) };
            yield return new object[] { false, new CreateVideoCommand(It.IsAny<string>(), validTile, tooLongDescription) };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}
