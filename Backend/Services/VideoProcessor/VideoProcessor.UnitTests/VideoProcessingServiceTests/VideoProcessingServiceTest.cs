using Infrastructure;
using Infrastructure.TransactionalEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SharedKernel.Exceptions;
using System.Reflection;
using UnitTestingUtilities;
using VideoProcessor.Application.BackgroundTasks;
using VideoProcessor.Application.BackgroundTasks.Processors.FileDownloaders;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoPreviewThumbnailGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoThumbnailGenerators;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Infrastructure;
using VideoProcessor.Domain.Contracts;
using VideoProcessor.Domain.DomainEvents;
using VideoProcessor.Domain.Models;
using Xunit.Abstractions;
using static VideoProcessor.Application.BackgroundTasks.VideoProcessingService;

namespace VideoProcessor.UnitTests.VideoProcessingServiceTests {
    public class VideoProcessingServiceTest {

        private readonly ITestOutputHelper _output;
        private readonly IOptions<VideoProcessorConfiguration> _config;
        private readonly ILogger<VideoProcessingService> _logger;

        public VideoProcessingServiceTest (ITestOutputHelper output) {
            _output = output;

            _config = new OptionsMock<VideoProcessorConfiguration>(new VideoProcessorConfiguration {
                MaxRetryCount = 5,
                ThumbnailPositions = new List<ThumbnailPositionConfiguration>(),
                VideoProcessingSteps = new List<VideoProcessingStepConfiguration> {
                    new VideoProcessingStepConfiguration{
                        Label = "144P",
                        Height = 144
                    }
                }
            });

            _logger = new LoggerMock<VideoProcessingService>(output, LogLevel.Information);
        }


        private static Task InvokeProcesVideoAsync (VideoProcessingService service, CancellationToken cancellationToken) {
            var methodInfo = typeof(VideoProcessingService).GetMethod("ProcessVideoAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var task = (Task)methodInfo.Invoke(service, new object[] { cancellationToken })!;
            return task;
        }

        [Fact]
        public async void VideoProcessingService_ShouldSucceed_WhenNoVideoIsPolled () {
            var serviceProvider = new ServiceProviderMock((services) => {
                services.AddScoped<IVideoRepository>(() => {
                    var mock = new Mock<IVideoRepository>();
                    mock.Setup(x => x.GetVideosToProcessAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(Enumerable.Empty<Video>()));

                    return mock.Object;
                });

                services.AddScoped<IUnitOfWork>(() => new Mock<UnitOfWorkMock>().Object);
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async void VideoProcessing_ShouldComplete_WhenAllProcessingSucceed () {
            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;
            ServiceProviderMock serviceProvider = CreateServiceProviderMock(video);

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingThumbnailsAddedDomainEvent>()), Times.AtLeastOnce());
            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingVideoAddedDomainEvent>()), Times.AtLeastOnce());
            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Once());
            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void VideoProcessing_ShouldFailOrRetry_WhenVideoFileCannotBeDownloaded (bool isTransientError) {
            ExceptionIdentifiers.Register(null);

            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;

            var serviceProvider = CreateServiceProviderMock(video, (services) => {
                services.AddScoped<IFileDownloader>(() => {
                    var mock = new Mock<IFileDownloader>();
                    mock.Setup(x => x.DownloadVideoAsync(video, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(isTransientError ? new TransientException() : new Exception());
                    return mock.Object;
                });
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Never());

            if (isTransientError) {
                Assert.NotEqual(0, video.RetryCount);
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
            } else {
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Once());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void VideoProcessing_ShouldFailOrRetry_WhenVideoInfoCannotBeGenerated (bool isTransientError) {
            ExceptionIdentifiers.Register(null);

            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;

            var serviceProvider = CreateServiceProviderMock(video, (services) => {
                services.AddScoped<IVideoInfoGenerator>(() => {
                    var mock = new Mock<IVideoInfoGenerator>();
                    mock.Setup(x => x.GenerateAsync(video, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(isTransientError ? new TransientException() : new Exception());
                    return mock.Object;
                });
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Never());

            if (isTransientError) {
                Assert.NotEqual(0, video.RetryCount);
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
            } else {
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Once());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void VideoProcessing_ShouldFailOrRetry_WhenVideoThumbnailCannotBeGenerated (bool isTransientError) {
            ExceptionIdentifiers.Register(null);

            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;

            var serviceProvider = CreateServiceProviderMock(video, (services) => {
                services.AddScoped<IVideoThumbnailGenerator>(() => {
                    var mock = new Mock<IVideoThumbnailGenerator>();

                    mock.Setup(x =>
                        x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).ThrowsAsync(isTransientError ? new TransientException() : new Exception());

                    return mock.Object;
                });
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Never());

            if (isTransientError) {
                Assert.NotEqual(0, video.RetryCount);
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
            } else {
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Once());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void VideoProcessing_ShouldFailOrRetry_WhenVideoPreviewThumbnailCannotBeGenerated (bool isTransientError) {
            ExceptionIdentifiers.Register(null);

            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;

            var serviceProvider = CreateServiceProviderMock(video, (services) => {
                services.AddScoped<IVideoPreviewThumbnailGenerator>(() => {
                    var mock = new Mock<IVideoPreviewThumbnailGenerator>();

                    mock.Setup(
                        x => x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).ThrowsAsync(isTransientError ? new TransientException() : new Exception());

                    return mock.Object;
                });
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Never());

            if (isTransientError) {
                Assert.NotEqual(0, video.RetryCount);
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
            } else {
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Once());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void VideoProcessing_ShouldFailOrRetry_WhenVideoCannotBeGenerated (bool isTransientError) {
            ExceptionIdentifiers.Register(null);

            var videoMock = new Mock<Video>(
                new object[] {
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _config.Value.VideoProcessingSteps.Select(
                        x=> VideoProcessingStep.Create(x.Label, x.Height)
                    ).ToList()
                });

            var video = videoMock.Object;

            var serviceProvider = CreateServiceProviderMock(video, (services) => {
                services.AddScoped<IVideoGenerator>(() => {
                    var mock = new Mock<IVideoGenerator>();

                    mock.Setup(x =>
                        x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<VideoProcessingStep>(),
                            It.IsAny<bool>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).ThrowsAsync(isTransientError ? new TransientException() : new Exception());

                    return mock.Object;
                });
            });

            var service = new VideoProcessingService(serviceProvider, _config, _logger);

            var task = InvokeProcesVideoAsync(service, default);

            await task;

            Assert.True(task.IsCompletedSuccessfully);

            videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingCompleteDomainEvent>()), Times.Never());

            if (isTransientError) {
                Assert.NotEqual(0, video.RetryCount);
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Never());
            } else {
                videoMock.Verify(x => x.AddDomainEvent(It.IsAny<VideoProcessingFailedDomainEvent>()), Times.Once());
            }
        }

        private ServiceProviderMock CreateServiceProviderMock (Video video, Action<IServiceProviderMockConfigurator>? configure = null) {
            return new ServiceProviderMock((services) => {
                services.Configure(_config.Value);

                services.AddScoped<IVideoRepository>(() => {
                    var mock = new Mock<IVideoRepository>();
                    mock.Setup(x => x.GetVideosToProcessAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new List<Video> { video }.AsEnumerable()));

                    mock.Setup(x => x.GetVideoByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(video)!);

                    return mock.Object;
                });

                services.AddScoped<ITempDirectoryRepository>(() => {
                    var mock = new Mock<ITempDirectoryRepository>();
                    return mock.Object;
                });

                services.AddScoped<IFileDownloader>(() => {
                    var mock = new Mock<IFileDownloader>();
                    return mock.Object;
                });

                services.AddScoped<IVideoInfoGenerator>(() => {
                    var mock = new Mock<IVideoInfoGenerator>();
                    return mock.Object;
                });

                services.AddScoped<IVideoThumbnailGenerator>(() => {
                    var mock = new Mock<IVideoThumbnailGenerator>();

                    mock.Setup(x =>
                        x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).Returns(Task.FromResult(new List<VideoThumbnail>()));

                    return mock.Object;
                });

                services.AddScoped<IVideoPreviewThumbnailGenerator>(() => {
                    var mock = new Mock<IVideoPreviewThumbnailGenerator>();

                    mock.Setup(
                        x => x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).Returns(Task.FromResult(
                        VideoPreviewThumbnail.Create(
                            It.IsAny<Guid>(),
                            It.IsAny<int>(),
                            It.IsAny<int>(),
                            It.IsAny<float>(),
                            It.IsAny<string>()
                        )
                    ));

                    return mock.Object;
                });

                services.AddScoped<IVideoGenerator>(() => {
                    var mock = new Mock<IVideoGenerator>();

                    mock.Setup(x =>
                        x.GenerateAsync(
                            video,
                            It.IsAny<VideoInfo>(),
                            It.IsAny<string>(),
                            It.IsAny<VideoProcessingStep>(),
                            It.IsAny<bool>(),
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()
                        )
                    ).Returns(Task.FromResult(
                        ProcessedVideo.Create(
                            It.IsAny<Guid>(),
                            It.IsAny<string>(),
                            It.IsAny<int>(),
                            It.IsAny<int>(),
                            It.IsAny<long>(),
                            It.IsAny<int>(),
                            It.IsAny<string>()
                        )
                    )!);

                    return mock.Object;
                });

                services.AddScoped<ILogger<VideoProcessingLock>>(() => new LoggerMock<VideoProcessingLock>(_output, LogLevel.Information));
                services.AddScoped<ITransactionalEventsContext>(() => new Mock<ITransactionalEventsContext>().Object);
                services.AddScoped<IUnitOfWork>(() => new Mock<UnitOfWorkMock>().Object);

                configure?.Invoke(services);
            });
        }

    }
}
