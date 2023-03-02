using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Users.API.Application.Commands;
using Users.API.Application.DtoModels;
using Users.API.Application.Queries;

namespace Users.API.Controller {
    [ApiController, Route("api/v1/[controller]")]
    public class UsersController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UsersController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("Thumbnail/UploadToken"), Authorize(Policy = "user")]
        public async Task<ActionResult<GetThumbnailUploadTokenResponseDto>> GetThumbnailUploadToken (CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetThumbnailUploadTokenQuery(GetUserId()), cancellationToken));
        }

        [HttpGet("Banner/UploadToken"), Authorize(Policy = "user")]
        public async Task<ActionResult<GetBannerUploadTokenResponseDto>> GetBannerUploadToken (CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetBannerUploadTokenQuery(GetUserId()), cancellationToken));
        }

        [HttpPut, Authorize(Policy = "user")]
        public async Task<IActionResult> UpdateUser (UpdateUserRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(
                new UpdateUserCommand(GetUserId(), request.UpdateBasicInfo, request.UpdateImages, request.UpdateLayout),
                cancellationToken);
            return Ok();
        }


        [HttpGet("HandleCheck"), Authorize(Policy = "user")]
        public async Task<IActionResult> CheckForHandleAvailability ([FromQuery] string handle, CancellationToken cancellationToken) {
            bool available = await _mediator.Send(
                new CheckForHandleAvailabilityQuery(handle),
                cancellationToken);
            return Ok(available);
        }

        [HttpPost, Authorize(Policy = "user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid display name")]
        [SwaggerResponse(StatusCodes.Status409Conflict, Description = "User already exists")]
        public async Task<IActionResult> CreateUser (CreateUserProfileRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new CreateUserProfileCommand(GetUserId(), request.DisplayName, request.ThumbnailToken), cancellationToken);
            return Ok();
        }

        [HttpGet, Authorize(Policy = "user")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserDataDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User not found")]
        public async Task<ActionResult<UserDataDto>> GetUserData (CancellationToken cancellationToken) {
            var getUserProfileTask = _mediator.Send(new GetUserProfileQuery(GetUserId(), null), cancellationToken);
            var getUserChannelTask = _mediator.Send(new GetUserChannelQuery(GetUserId(), null, null), cancellationToken);

            await Task.WhenAll(getUserChannelTask, getUserProfileTask);

            var userProfile = getUserProfileTask.Result;
            var userChannel = getUserChannelTask.Result;

            return Ok(new UserDataDto {
                UserProfile = _mapper.Map<DetailedUserProfileDto>(userProfile, options => options.Items["resolveUrl"] = true),
                UserChannel = _mapper.Map<DetailedUserChannelDto>(userChannel, options => options.Items["resolveUrl"] = true)
            });
        }

        private string? GetOptionalUserId () {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        }

        private string GetUserId () {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
            }
            return userId;
        }

    }
}
