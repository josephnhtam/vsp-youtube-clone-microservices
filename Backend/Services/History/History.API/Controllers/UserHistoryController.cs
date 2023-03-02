using History.API.Application.Commands;
using History.API.Application.DtoModels;
using History.API.Application.Queries;
using History.Infrastructure.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Security.Claims;

namespace History.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class UserHistoryController : ControllerBase {

        private readonly IMediator _mediator;

        public UserHistoryController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost("Record")]
        public async Task<IActionResult> RecordVideoView ([FromQuery] Guid videoId, CancellationToken cancellationToken) {
            await _mediator.Send(new RecordVideoViewCommand(GetOptionalUserId(), videoId), cancellationToken);
            return Ok();
        }

        [HttpGet, Authorize(Policy = "user")]
        public async Task<ActionResult<SearchResponseDto>> SearchUserWatchHistory ([FromQuery] UserWatchHistorySearchRequestDto request, CancellationToken cancellationToken) {
            var searchParams = new UserWatchHistorySearchParameters {
                UserId = GetUserId(),
                Query = request.Query,
                Pagination = new Pagination(request.Page ?? 1, request.PageSize ?? 50),
                PeriodRange = new PeriodRange(request.From, request.To)
            };

            return Ok(await _mediator.Send(new SearchUserWatchHistoryQuery(searchParams), cancellationToken));
        }

        [HttpDelete, Authorize(Policy = "user")]
        public async Task<IActionResult> ClearUserWatchHistory (CancellationToken cancellationToken) {
            await _mediator.Send(new ClearUserWatchHistoryCommand(GetUserId()), cancellationToken);
            return Ok();
        }

        [HttpDelete("Video"), Authorize(Policy = "user")]
        public async Task<IActionResult> RemoveVideoFromUserWatchHistory ([FromQuery] Guid videoId, CancellationToken cancellationToken) {
            await _mediator.Send(new RemoveVideoFromUserWatchHistoryCommand(GetUserId(), videoId), cancellationToken);
            return Ok();
        }

        [HttpPost("EnableRecording"), Authorize(Policy = "user")]
        public async Task<IActionResult> EnableRecordUserWatchHistory (SwitchRecordUserWatchHistoryRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new EnableRecordUserWatchHistoryCommand(GetUserId(), request.Enable), cancellationToken);
            return Ok();
        }

        [HttpGet("Settings"), Authorize(Policy = "user")]
        public async Task<ActionResult<UserHistorySettingsDto>> GetUserHistorySettings (CancellationToken cancellationToken) {
            var settings = await _mediator.Send(new GetUserHistorySettingsQuery(GetUserId()), cancellationToken);

            if (settings == null) {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Ok(settings);
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
