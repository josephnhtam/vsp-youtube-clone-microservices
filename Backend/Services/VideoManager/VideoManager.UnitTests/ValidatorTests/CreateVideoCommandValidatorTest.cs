using VideoManager.API.Application.Commands.Validators;
using VideoManager.API.Commands;
using VideoManager.UnitTests.Data;

namespace VideoManager.UnitTests.ValidatorTests {
    public class CreateVideoCommandValidatorTest {

        private readonly CreateVideoCommandValidator _validator;

        public CreateVideoCommandValidatorTest () {
            _validator = new CreateVideoCommandValidator();
        }

        [Theory]
        [ClassData(typeof(CreateVideoCommandData))]
        public void Validation_ShouldBeValid_WhenCommandIsValid (bool isValid, CreateVideoCommand command) {
            var result = _validator.Validate(command);
            Assert.Equal(result.IsValid, isValid);
        }

    }
}
