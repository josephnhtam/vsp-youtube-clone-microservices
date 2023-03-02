using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Users.API.Application.DtoModels;
using Users.API.Application.Queries;
using Users.Domain.Models;

namespace Users.API.Controller {
    [ApiController, Route("api/v1/[controller]")]
    public class UserProfilesController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UserProfilesController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User profile not found")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile (string? userId = null, string? handle = null, CancellationToken cancellationToken = default) {
            var userProfile = await _mediator.Send(new GetUserProfileQuery(userId, handle), cancellationToken);
            return Ok(_mapper.Map<UserProfileDto>(userProfile, options => options.Items["resolveUrl"] = true));
        }

        [HttpGet("Self"), Authorize(Policy = "user")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PrivateUserProfileDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User profile not found")]
        public async Task<ActionResult<PrivateUserProfileDto>> GetPrivateUserProfile (CancellationToken cancellationToken) {
            UserProfile? userProfile = await _mediator.Send(new GetUserProfileQuery(GetUserId(), null, false), cancellationToken);
            if (userProfile == null) return NotFound();
            return Ok(_mapper.Map<PrivateUserProfileDto>(userProfile, options => options.Items["resolveUrl"] = true));
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
