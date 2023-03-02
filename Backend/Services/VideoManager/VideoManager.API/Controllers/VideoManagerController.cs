using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Security.Claims;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.Queries;
using VideoManager.API.Commands;

namespace VideoManager.API.Controllers {
    [ApiController, Route("api/v1/[controller]"), Authorize(Policy = "user")]
    public class VideoManagerController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public VideoManagerController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("Videos")]
        public async Task<ActionResult<VideoDto>> CreateVideo (CreateVideoRequestDto request, CancellationToken cancellationToken) {
            var video = await _mediator.Send(new CreateVideoCommand(GetUserId(), request.Title, request.Description), cancellationToken);
            return Ok(_mapper.Map<VideoDto>(video, options => options.Items["resolveUrl"] = true));
        }

        [HttpGet("Videos/{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo (Guid videoId, CancellationToken cancellationToken) {
            var video = await _mediator.Send(new GetVideoQuery(videoId), cancellationToken);
            return Ok(_mapper.Map<VideoDto>(video, options => options.Items["resolveUrl"] = true));
        }

        [HttpGet("Videos")]
        public async Task<ActionResult<GetVideosResponseDto>> GetVideos ([FromQuery] GetVideosRequestDto request, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetVideosQuery(GetUserId(), request), cancellationToken));
        }

        [HttpGet("Videos/{videoId}/UploadToken")]
        public async Task<ActionResult<VideoUploadTokenResponseDto>> GetVideoUploadToken (Guid videoId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetVideoUploadTokenQuery(GetUserId(), videoId), cancellationToken));
        }

        [HttpPut("Videos")]
        public async Task<ActionResult<VideoDto>> SetVideoInfo (SetVideoInfoRequestDto request, CancellationToken cancellationToken) {
            var video = await _mediator.Send(new SetVideoInfoCommand {
                CreatorId = GetUserId(),
                VideoId = request.VideoId,
                SetVideoBasicInfo = request.SetBasicInfo != null ? _mapper.Map<SetVideoBasicInfoCommand>(request.SetBasicInfo) : null,
                SetVideoVisibilityInfo = request.SetVisibilityInfo != null ? _mapper.Map<SetVideoVisibilityInfoCommand>(request.SetVisibilityInfo) : null
            }, cancellationToken);

            return video != null ? Ok(_mapper.Map<VideoDto>(video, options => options.Items["resolveUrl"] = true)) : NotFound();
        }

        [HttpDelete("Videos/{videoId}")]
        public async Task<ActionResult<VideoDto>> UnregisterVideo (Guid videoId, CancellationToken cancellationToken) {
            await _mediator.Send(new UnregisterVideoCommand(GetUserId(), videoId), cancellationToken);
            return Ok();
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
