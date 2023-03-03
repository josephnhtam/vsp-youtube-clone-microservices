using Moq;
using VideoManager.API.Application.Commands.Validators;
using VideoManager.API.Commands;
using VideoManager.Domain.Models;
using VideoManager.Domain.Rules.Videos;
using VideoManager.UnitTests.Data;

namespace VideoManager.UnitTests.ValidatorTests {
    public class SetVideoInfoCommandValidatorTest {

        private readonly SetVideoInfoCommandValidator _validator;

        public SetVideoInfoCommandValidatorTest () {
            _validator = new SetVideoInfoCommandValidator();
        }

        [Theory]
        [ClassData(typeof(SetVideoInfoCommandData))]
        public void Validation_ShouldBeValid_WhenCommandIsValid (bool isValid, SetVideoInfoCommand command) {
            var result = _validator.Validate(command);
            Assert.Equal(result.IsValid, isValid);
        }

    }
}
