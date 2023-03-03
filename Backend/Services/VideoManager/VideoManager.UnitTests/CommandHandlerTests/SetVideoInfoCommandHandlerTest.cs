using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Exceptions;
using VideoManager.API.Application.Commands.Handlers;
using VideoManager.API.Commands;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.DomainEvents;
using VideoManager.Domain.Models;
using VideoManager.UnitTests.Data;
using VideoManager.UnitTests.Mocks;

namespace VideoManager.UnitTests.CommandHandlerTests {
    public class SetVideoInfoCommandHandlerTest {

        private Mock<IVideoRepository> _videoRepositoryMock;
        private Mock<UnitOfWorkMock> _unitOfWorkMock;
        private Mock<ILogger<SetVideoInfoCommandHandler>> _loggerMock;

        public SetVideoInfoCommandHandlerTest () {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<UnitOfWorkMock>();
            _loggerMock = new Mock<ILogger<SetVideoInfoCommandHandler>>();
        }

        [Theory]
        [ClassData(typeof(SetVideoInfoCommandData))]
        public async Task Handle_ShouldSucceed_WhenCommandIsValid (bool isValid, SetVideoInfoCommand command) {
            var videoMock = new Mock<Video>(It.IsAny<Guid>(), It.IsAny<Guid>(), command.CreatorId, "Valid Title", "Valid Description");

            _videoRepositoryMock.Setup(x => x.GetVideoByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(videoMock.Object)!);

            var handler = new SetVideoInfoCommandHandler(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            if (isValid) {
                var task = handler.Handle(command, default);
                await task;

                // Handle should succeed
                Assert.True(task.IsCompletedSuccessfully);

                // VideoInfoUpdatedDomainEvent should be added
                videoMock.Verify(x => x.AddUniqueDomainEvent(It.IsAny<VideoInfoUpdatedDomainEvent>()));

                // IUnitOfWork.CommitAsync should be called
                _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            } else {
                // BusinessRuleValidationException should be thrown
                await Assert.ThrowsAnyAsync<BusinessRuleValidationException>(() => handler.Handle(command, default));

                // IUnitOfWork.CommitAsync should not be called
                _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            }
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenVideoNotFound () {
            var command = ValidUpdateVideoInfoCommand();

            _videoRepositoryMock.Setup(x => x.GetVideoByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult((Video?)null));

            _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException());

            var handler = new SetVideoInfoCommandHandler(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            // Exception should be thrown
            var ex = await Assert.ThrowsAnyAsync<AppException>(() => handler.Handle(command, default));
            Assert.Equal(ex.StatusCode, StatusCodes.Status404NotFound);

            // IUnitOfWork.CommitAsync should not be called
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenCreatorIdNotMatched () {
            var command = ValidUpdateVideoInfoCommand();
            var videoMock = new Mock<Video>(It.IsAny<Guid>(), It.IsAny<Guid>(), Guid.NewGuid().ToString(), "Valid Title", "Valid Description");

            _videoRepositoryMock.Setup(x => x.GetVideoByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(videoMock.Object)!);

            var handler = new SetVideoInfoCommandHandler(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            // Exception should be thrown
            var ex = await Assert.ThrowsAnyAsync<AppException>(() => handler.Handle(command, default));
            Assert.Equal(ex.StatusCode, StatusCodes.Status403Forbidden);

            // IUnitOfWork.CommitAsync should not be called
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNotPersisted () {
            var command = ValidUpdateVideoInfoCommand();
            var videoMock = new Mock<Video>(It.IsAny<Guid>(), It.IsAny<Guid>(), command.CreatorId, "Valid Title", "Valid Description");

            _videoRepositoryMock.Setup(x => x.GetVideoByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(videoMock.Object)!);

            _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException());

            var handler = new SetVideoInfoCommandHandler(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            // Exception should be thrown
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => handler.Handle(command, default));

            // VideoInfoUpdatedDomainEvent should be added
            videoMock.Verify(x => x.AddUniqueDomainEvent(It.IsAny<VideoInfoUpdatedDomainEvent>()));

            // IUnitOfWork.CommitAsync should be called
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        private static SetVideoInfoCommand ValidUpdateVideoInfoCommand () {
            return new SetVideoInfoCommand {
                SetVideoBasicInfo = new() {
                    Title = "Valid Title",
                    Description = "Valid Description",
                    Tags = "Tag1,Tag2"
                },
                SetVideoVisibilityInfo = new() {
                    Visibility = It.IsAny<VideoVisibility>()
                }
            };
        }

    }
}
