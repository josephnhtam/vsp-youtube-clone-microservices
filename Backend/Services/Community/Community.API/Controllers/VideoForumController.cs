using AutoMapper;
using Community.API.Application.Commands;
using Community.API.Application.DtoModels;
using Community.API.Application.Queries;
using Community.Domain.Models;
using Community.Domain.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Security.Claims;

namespace Community.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class VideoForumController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public VideoForumController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("Comments"), Authorize(Policy = "user")]
        public async Task<ActionResult<CommentAddedResponseDto>> AddComment (AddCommentRequestDto request, CancellationToken cancellationToken) {
            var commentAdded = await _mediator.Send(new AddVideoCommentCommand(request.VideoId, GetUserId(), request.Comment), cancellationToken);
            return Ok(new CommentAddedResponseDto {
                Comment = _mapper.Map<VideoCommentDto>(commentAdded, options => options.Items["resolveUrl"] = true)
            });
        }

        [HttpPost("Comments/Replies"), Authorize(Policy = "user")]
        public async Task<ActionResult<CommentAddedResponseDto>> Reply (ReplyToCommentRequestDto request, CancellationToken cancellationToken) {
            var commentAdded = await _mediator.Send(new AddVideoCommentCommand(request.CommentId, GetUserId(), request.Comment), cancellationToken);
            return Ok(new CommentAddedResponseDto {
                Comment = _mapper.Map<VideoCommentDto>(commentAdded, options => options.Items["resolveUrl"] = true)
            });
        }

        [HttpPost("Comments/Votes"), Authorize(Policy = "user")]
        public async Task<IActionResult> VoteVideoComment (VoteVideoCommentRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new VoteVideoCommentCommand(request.CommentId, GetUserId(), request.VoteType), cancellationToken);
            return Ok();
        }

        [HttpGet("Comments/Votes"), Authorize(Policy = "user")]
        public async Task<ActionResult<UserVideoCommentVotesDto>> GetUserVideoVotes (Guid videoId, CancellationToken cancellationToken) {
            var votes = await _mediator.Send(new GetVotedVideoCommentIdsQuery(videoId, GetUserId()), cancellationToken);

            return Ok(new UserVideoCommentVotesDto {
                LikedCommentIds = votes.Where(x => x.VoteType == VoteType.Like).Select(x => x.CommentId),
                DislikedCommentIds = votes.Where(x => x.VoteType == VoteType.Dislike).Select(x => x.CommentId)
            });
        }

        [HttpPut("Comments"), Authorize(Policy = "user")]
        public async Task<IActionResult> EditVideoComments (EditCommentRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new EditVideoCommentCommand(GetUserId(), request.CommentId, request.Comment), cancellationToken);
            return Ok();
        }

        [HttpDelete("Comments"), Authorize(Policy = "user")]
        public async Task<IActionResult> DeleteVideoComments (long commentId, CancellationToken cancellationToken) {
            await _mediator.Send(new DeleteVideoCommentCommand(GetUserId(), commentId), cancellationToken);
            return Ok();
        }

        [HttpGet("Comments")]
        public async Task<ActionResult<List<VideoCommentDto>>> GetRootVideoComments (Guid videoId, long maxTimestamp, int page, int pageSize, VideoCommentSort sort, CancellationToken cancellationToken) {
            var maxDate = DateTimeOffset.FromUnixTimeMilliseconds(maxTimestamp).ToUniversalTime();
            var comments = await _mediator.Send(new GetRootVideoCommentsQuery(videoId, maxDate, page, pageSize, sort), cancellationToken);
            return Ok(_mapper.Map<List<VideoCommentDto>>(comments, options => options.Items["resolveUrl"] = true));
        }

        [HttpGet("Comments/Replies")]
        public async Task<ActionResult<List<VideoCommentDto>>> GetVideoCommentReplies (long commentId, int page, int pageSize, CancellationToken cancellationToken) {
            var comments = await _mediator.Send(new GetVideoCommentRepliesQuery(commentId, page, pageSize), cancellationToken);
            return Ok(_mapper.Map<List<VideoCommentDto>>(comments, options => options.Items["resolveUrl"] = true));
        }

        [HttpGet]
        public async Task<ActionResult<GetVideoForumResponseDto>> GetVideoForum (Guid videoId, int pageSize, VideoCommentSort sort, CancellationToken cancellationToken) {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _mediator.Send(new GetVideoForumQuery(videoId, pageSize, sort, userId), cancellationToken));
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
