using Application.Handlers;
using AutoMapper;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using Users.API.Application.DtoModels;
using Users.API.Application.Services;
using Users.API.Application.Utilities;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Commands.Handlers {
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserChannelRepository _userChannelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler (
            IUserProfileRepository userProfileRepository,
            IUserChannelRepository userChannelRepository,
            IUnitOfWork unitOfWork,
            IImageValidator imageValidator,
            IMapper mapper,
            ILogger<UpdateUserCommandHandler> logger) {
            _userProfileRepository = userProfileRepository;
            _userChannelRepository = userChannelRepository;
            _unitOfWork = unitOfWork;
            _imageValidator = imageValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateUserCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                if (RequireUserProfileUpdate(request)) {
                    var userProfile = await _userProfileRepository.GetUserProfileByIdAsync(request.UserId, true, cancellationToken);

                    if (userProfile == null) {
                        throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
                    }

                    if (request.UpdateBasicInfo != null) {
                        if (request.UpdateBasicInfo.Handle != null) {
                            var handleUserProfile = await _userProfileRepository
                                .GetUserProfileByHandleAsync(request.UpdateBasicInfo.Handle, false, cancellationToken);

                            if (handleUserProfile != null && handleUserProfile.Id != request.UserId) {
                                throw new AppException("This handle is already in used", null, StatusCodes.Status400BadRequest);
                            }
                        }

                        UpdateUserProfileBasicInfo(userProfile, request.UpdateBasicInfo);
                    }

                    if (request.UpdateImages != null) {
                        UpdateUserProfileImage(userProfile, request.UpdateImages);
                    }
                }

                if (RequireUserChannelUpdate(request)) {
                    var userChannel = await _userChannelRepository.GetUserChannelByIdAsync(request.UserId, false, true, cancellationToken);

                    if (userChannel == null) {
                        throw new AppException("User channel not found", null, StatusCodes.Status404NotFound);
                    }

                    if (request.UpdateLayout != null) {
                        UpdateUserChannelLayout(userChannel, request.UpdateLayout);
                    }

                    if (request.UpdateImages != null) {
                        UpdateUserChannelImage(userChannel, request.UpdateImages);
                    }
                }

                try {
                    await _unitOfWork.CommitAsync(cancellationToken);
                } catch (Exception ex) when (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    throw new AppException("Invalid request", null, StatusCodes.Status400BadRequest);
                }
            });

            return Unit.Value;
        }

        private bool RequireUserProfileUpdate (UpdateUserCommand request) {
            return
                request.UpdateBasicInfo != null ||
                (request.UpdateImages != null &&
                 request.UpdateImages.ThumbnailChanged);
        }

        private bool RequireUserChannelUpdate (UpdateUserCommand request) {
            return
                request.UpdateLayout != null ||
                (request.UpdateImages != null &&
                 request.UpdateImages.BannerChanged);
        }

        private void UpdateUserProfileBasicInfo (UserProfile userProfile, UpdateUserProfileBasicInfo request) {
            userProfile.UpdateInfo(request.DisplayName, request.Description, request.Handle, request.Email);
        }

        private void UpdateUserProfileImage (UserProfile userProfile, UpdateUserBrandingImages request) {
            if (request.ThumbnailChanged) {
                if (request.NewThubmnailToken != null) {
                    var thumbnailFile = _imageValidator.ValidateImageToken(
                        request.NewThubmnailToken, Categories.UserUploadedThumbnail, userProfile.Id);

                    userProfile.UpdateThumbnail(thumbnailFile);
                } else {
                    userProfile.UpdateThumbnail(null);
                }
            }
        }

        private void UpdateUserChannelImage (UserChannel userChannel, UpdateUserBrandingImages request) {
            if (request.BannerChanged) {
                if (request.NewBannerToken != null) {
                    var bannerlFile = _imageValidator.ValidateImageToken(
                         request.NewBannerToken, Categories.UserUploadedBanner, userChannel.Id);

                    userChannel.UpdateBanner(bannerlFile);
                } else {
                    userChannel.UpdateBanner(null);
                }
            }
        }

        private void UpdateUserChannelLayout (UserChannel userChannel, UpdateUserChannelLayout request) {
            var channelSections = _mapper.Map<List<ChannelSection>>(request.ChannelSections);

            CheckIdDuplicate(channelSections);
            CheckTypeDuplicate<VideosSection>(channelSections);
            CheckTypeDuplicate<CreatedPlaylistsSection>(channelSections);

            userChannel.UpdateChannelLayout(
                request.UnsubscribedSpotlightVideoId,
                request.SubscribedSpotlightVideoId,
                channelSections);
        }

        private static void CheckIdDuplicate (List<ChannelSection> sections) {
            if (sections.GroupBy(x => x.Id).Any(x => x.Count() > 1)) {
                throw new AppException("Invalid channel sections", null, StatusCodes.Status400BadRequest);
            }
        }

        private static void CheckTypeDuplicate<TType> (List<ChannelSection> sections) where TType : ChannelSection {
            if (sections.OfType<TType>().Count() > 1) {
                throw new AppException("Invalid channel sections", null, StatusCodes.Status400BadRequest);
            }
        }

    }
}
