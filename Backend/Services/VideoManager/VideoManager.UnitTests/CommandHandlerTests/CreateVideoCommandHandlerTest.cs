using Domain.Exceptions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VideoManager.API.Application.Commands.Handlers;
using VideoManager.API.Commands;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;
using VideoManager.UnitTests.Data;

namespace VideoManager.UnitTests.CommandHandlerTests {
    public class CreateVideoCommandHandlerTest {

        private Mock<IUserProfileRepository> _userProfileRepositoryMock;
        private Mock<IVideoRepository> _videoRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreateVideoCommandHandler>> _loggerMock;

        public CreateVideoCommandHandlerTest () {
            _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateVideoCommandHandler>>();
        }

        [Theory]
        [ClassData(typeof(CreateVideoCommandData))]
        public async Task Handle_ShouldThrow_WhenCommandIsNotValid (bool isValid, CreateVideoCommand command) {
            var handler = new CreateVideoCommandHandler(
                _userProfileRepositoryMock.Object,
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            if (isValid) {
                var task = handler.Handle(command, default);
                await task;

                // Handle should succeed
                Assert.True(task.IsCompletedSuccessfully);

                // IVidoeRepository.AddVideoAsync should be called
                _videoRepositoryMock.Verify(x => x.AddVideoAsync(It.IsAny<Video>()), Times.Once());

                // IUnitOfWork.CommitAsync should be called
                _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            } else {
                // BusinessRuleValidationException should be thrown
                await Assert.ThrowsAnyAsync<BusinessRuleValidationException>(() => handler.Handle(command, default));

                // IVidoeRepository.AddVideoAsync should not be called
                _videoRepositoryMock.Verify(x => x.AddVideoAsync(It.IsAny<Video>()), Times.Never());

                // IUnitOfWork.CommitAsync should not be called
                _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            }
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNotPersisted () {
            _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException());

            var handler = new CreateVideoCommandHandler(
                _userProfileRepositoryMock.Object,
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            // Exception should be thrown
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => handler.Handle(ValidCreateVideoCommand(), default));

            // IVidoeRepository.AddVideoAsync should be called
            _videoRepositoryMock.Verify(x => x.AddVideoAsync(It.IsAny<Video>()), Times.Once());

            // IUnitOfWork.CommitAsync should be called
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        private static CreateVideoCommand ValidCreateVideoCommand () {
            return new CreateVideoCommand(It.IsAny<string>(), "Valid title", "Valid Description");
        }

    }
}
