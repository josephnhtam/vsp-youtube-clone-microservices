using Library.API.Application.Commands;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries;
using Library.Domain.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Security.Claims;

namespace Library.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class VideoLibraryController : ControllerBase {

        private readonly IMediator _mediator;

        public VideoLibraryController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet("{videoId}/Metadata")]
        public async Task<ActionResult<VideoMetadataDto>> GetMetadata (Guid videoId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetVideoMetadataQuery(videoId, GetOptionalUserId()), cancellationToken));
        }

        [HttpPost("Vote"), Authorize(Policy = "user")]
        public async Task<IActionResult> Vote (VoteVideoRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new VoteVideoCommand(GetUserId(), request.VideoId, request.VoteType), cancellationToken);
            return Ok();
        }

        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto?>> GetVideo (Guid videoId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetVideoQuery(videoId, true), cancellationToken));
        }

        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetVideosCount (string userId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetPublicVideosCountQuery(userId), cancellationToken));
        }

        [HttpGet("TotalViews")]
        public async Task<ActionResult<long>> GetVideoTotalViews (string userId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetPublicVideoTotalViewsQuery(userId), cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<GetVideosResponseDto>> GetVideos (string userId, int page = 1, int pageSize = 50, VideoSort sort = VideoSort.CreateDate, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetVideosQuery(userId, true, new Pagination(page, pageSize), sort), cancellationToken));
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
