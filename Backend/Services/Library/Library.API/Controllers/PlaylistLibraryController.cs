using Library.API.Application.Commands;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries;
using Library.Domain.Models;
using Library.Domain.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Security.Claims;

namespace Library.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class PlaylistLibraryController : ControllerBase {

        private readonly IMediator _mediator;

        public PlaylistLibraryController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPut("Items/Move"), Authorize(Policy = "user")]
        public async Task<IActionResult> MovePlaylistItem (MovePlaylistItemByIdRequestDto request, CancellationToken cancellationToken) {
            var userId = GetUserId();

            switch (request.Playlist.ToUpper()) {
                case "LL":
                case "DL": {
                        return BadRequest();
                    }
                case "WL": {
                        await _mediator.Send(new MoveWatchLaterPlaylistItemByIdCommand(userId, request.ItemId, request.PrecedingItemId), cancellationToken);
                        return Ok();
                    }
            }

            if (!Guid.TryParse(request.Playlist, out var playlistId)) return BadRequest();
            await _mediator.Send(new MovePlaylistItemByIdCommand(userId, playlistId, request.ItemId, request.PrecedingItemId), cancellationToken);
            return Ok();
        }

        [HttpPut("Items/MoveTo"), Authorize(Policy = "user")]
        public async Task<IActionResult> MovePlaylistItem (MovePlaylistItemRequestDto request, CancellationToken cancellationToken) {
            var userId = GetUserId();

            switch (request.Playlist.ToUpper()) {
                case "LL":
                case "DL": {
                        return BadRequest();
                    }
                case "WL": {
                        await _mediator.Send(new MoveWatchLaterPlaylistItemCommand(userId, request.ItemId, request.ToPosition), cancellationToken);
                        return Ok();
                    }
            }

            if (!Guid.TryParse(request.Playlist, out var playlistId)) return BadRequest();
            await _mediator.Send(new MovePlaylistItemCommand(userId, playlistId, request.ItemId, request.ToPosition), cancellationToken);
            return Ok();
        }

        [HttpPost("Items"), Authorize(Policy = "user")]
        public async Task<IActionResult> AddVideoToPlaylist (AddVideoToPlaylistRequestDto request, CancellationToken cancellationToken) {
            var userId = GetUserId();

            switch (request.Playlist.ToUpper()) {
                case "LL": {
                        await _mediator.Send(new VoteVideoCommand(userId, request.VideoId, VoteType.Like), cancellationToken);
                        return Ok();
                    }
                case "DL": {
                        await _mediator.Send(new VoteVideoCommand(userId, request.VideoId, VoteType.Dislike), cancellationToken);
                        return Ok();
                    }
                case "WL": {
                        await _mediator.Send(new AddVideoToWatchLaterPlaylistCommand(userId, request.VideoId), cancellationToken);
                        return Ok();
                    }
            }

            if (!Guid.TryParse(request.Playlist, out var playlistId)) return BadRequest();
            await _mediator.Send(new AddVideoToPlaylistCommand(userId, playlistId, request.VideoId), cancellationToken);
            return Ok();
        }

        [HttpDelete("Items/Video"), Authorize(Policy = "user")]
        public async Task<IActionResult> RemoveVideoFromPlaylist ([FromQuery] RemoveVideoFromPlaylistRequestDto request, CancellationToken cancellationToken) {
            var userId = GetUserId();

            switch (request.Playlist.ToUpper()) {
                case "LL":
                case "DL": {
                        await _mediator.Send(new VoteVideoCommand(userId, request.VideoId, VoteType.None), cancellationToken);
                        return Ok();
                    }
                case "WL": {
                        await _mediator.Send(new RemoveVideoFromWatchLaterPlaylistCommand(userId, request.VideoId), cancellationToken);
                        return Ok();
                    }
            }

            if (!Guid.TryParse(request.Playlist, out var playlistId)) return BadRequest();
            await _mediator.Send(new RemoveVideoFromPlaylistCommand(userId, playlistId, request.VideoId), cancellationToken);
            return Ok();
        }

        [HttpDelete("Items"), Authorize(Policy = "user")]
        public async Task<IActionResult> RemoveItemFromPlaylist ([FromQuery] RemoveItemFromPlaylistRequestDto request, CancellationToken cancellationToken) {
            var userId = GetUserId();

            switch (request.Playlist.ToUpper()) {
                case "LL":
                case "DL": {
                        return BadRequest();
                    }
                case "WL": {
                        await _mediator.Send(new RemoveItemFromWatchLaterPlaylistCommand(userId, request.ItemId), cancellationToken);
                        return Ok();
                    }
            }

            if (!Guid.TryParse(request.Playlist, out var playlistId)) return BadRequest();
            await _mediator.Send(new RemoveItemFromPlaylistCommand(userId, playlistId, request.ItemId), cancellationToken);
            return Ok();
        }

        [HttpGet("SimpleInfos/Self"), Authorize(Policy = "user")]
        public async Task<ActionResult<GetSimplePlaylistInfosResponseDto>> GetSimplePlaylistInfos (int? page = null, int? pageSize = null, CancellationToken cancellationToken = default) {
            var pagination = page.HasValue && pageSize.HasValue ? new Pagination(page.Value, pageSize.Value) : null;
            return Ok(await _mediator.Send(new GetAllSimplePlaylistInfosQuery(GetUserId(), pagination), cancellationToken));
        }

        [HttpGet("Infos/Self"), Authorize(Policy = "user")]
        public async Task<ActionResult<GetPlaylistInfosResponseDto>> GetUserPlaylistInfos (int? page = null, int? pageSize = null, CancellationToken cancellationToken = default) {
            var pagination = page.HasValue && pageSize.HasValue ? new Pagination(page.Value, pageSize.Value) : null;
            return Ok(await _mediator.Send(new GetAllPlaylistInfosQuery(GetUserId(), pagination), cancellationToken));
        }

        [HttpGet("Infos/User")]
        public async Task<ActionResult<GetPlaylistInfosResponseDto>> GetCreatedPlaylistInfos (string userId, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default) {
            var pagination = page.HasValue && pageSize.HasValue ? new Pagination(page.Value, pageSize.Value) : null;
            return Ok(await _mediator.Send(new GetCreatedPlaylistInfosQuery(userId, true, pagination), cancellationToken));
        }

        [HttpPost("Infos")]
        public async Task<ActionResult<List<PlaylistInfoDto>>> GetPublicPlaylistInfosByIds (GetPublicPlaylistInfosByIdsRequestDto request, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetPublicPlaylistInfosByIdsQuery(GetOptionalUserId(), request.PlaylistIds), cancellationToken));
        }

        [HttpPost("SimpleInfos")]
        public async Task<ActionResult<List<SimplePlaylistInfoDto>>> GetPublicSimplePlaylistInfosByIds (GetPublicPlaylistInfosByIdsRequestDto request, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetPublicSimplePlaylistInfosByIdsQuery(request.PlaylistIds), cancellationToken));
        }

        [HttpGet("WithVideo"), Authorize(Policy = "user")]
        public async Task<ActionResult<PlaylistsWithVideoDto>> GetPlaylistsWithVideo (Guid videoId, CancellationToken cancellationToken) {
            return await _mediator.Send(new GetPlaylistsWithVideoQuery(GetUserId(), videoId), cancellationToken);
        }

        [HttpPost, Authorize(Policy = "user")]
        public async Task<ActionResult<Guid>> CreatePlaylist (CreatePlaylistRequestDto request, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new CreatePlaylistCommand(GetUserId(), request.Title, request.Description, request.Visibility), cancellationToken));
        }

        [HttpDelete, Authorize(Policy = "user")]
        public async Task<ActionResult<Guid>> RemovePlaylist ([FromQuery] RemovePlaylistRequestDto request, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new RemovePlaylistCommand(GetUserId(), request.PlaylistId), cancellationToken));
        }

        [HttpPut, Authorize(Policy = "user")]
        public async Task<IActionResult> UpdatePlaylist (UpdatePlaylistRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new UpdatePlaylistCommand(GetUserId(), request.PlaylistId, request.Title, request.Description, request.Visibility), cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist (string playlist, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default) {
            var playlistUpper = playlist.ToUpper();

            switch (playlistUpper) {
                case "LL": {
                        return Ok(await _mediator.Send(new GetLikedPlaylistQuery(GetUserId(), new Pagination(page, pageSize)), cancellationToken));
                    }
                case "DL": {
                        return Ok(await _mediator.Send(new GetDislikedPlaylistQuery(GetUserId(), new Pagination(page, pageSize)), cancellationToken));
                    }
                case "WL": {
                        return Ok(await _mediator.Send(new GetWatchLaterPlaylistQuery(GetUserId(), new Pagination(page, pageSize)), cancellationToken));
                    }
            }

            if (playlistUpper.StartsWith("VIDEOS-")) {
                string videosUserId = playlist.Substring(7);
                return Ok(await _mediator.Send(new GetVideosPlaylistQuery(GetOptionalUserId(), videosUserId, new Pagination(page, pageSize)), cancellationToken));
            } else {
                if (!Guid.TryParse(playlist, out var playlistId)) return BadRequest();
                return Ok(await _mediator.Send(new GetPlaylistQuery(GetOptionalUserId(), playlistId, new Pagination(page, pageSize)), cancellationToken));
            }
        }

        [HttpGet("Near")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist (string playlist, Guid? videoId = null, int? index = null, int pageSize = 50, CancellationToken cancellationToken = default) {
            var playlistUpper = playlist.ToUpper();

            switch (playlistUpper) {
                case "LL": {
                        return Ok(await _mediator.Send(new GetLikedPlaylistQuery(GetUserId(), new IndexPagination(videoId, index, pageSize)), cancellationToken));
                    }
                case "DL": {
                        return Ok(await _mediator.Send(new GetDislikedPlaylistQuery(GetUserId(), new IndexPagination(videoId, index, pageSize)), cancellationToken));
                    }
                case "WL": {
                        return Ok(await _mediator.Send(new GetWatchLaterPlaylistQuery(GetUserId(), new IndexPagination(videoId, index, pageSize)), cancellationToken));
                    }
            }

            if (playlistUpper.StartsWith("VIDEOS-")) {
                string videosUserId = playlist.Substring(7);
                return Ok(await _mediator.Send(new GetVideosPlaylistQuery(GetOptionalUserId(), videosUserId, new IndexPagination(videoId, index, pageSize)), cancellationToken));
            } else {
                if (!Guid.TryParse(playlist, out var playlistId)) return BadRequest();
                return Ok(await _mediator.Send(new GetPlaylistQuery(GetOptionalUserId(), playlistId, new IndexPagination(videoId, index, pageSize)), cancellationToken));
            }
        }

        [HttpDelete("Ref"), Authorize(Policy = "user")]
        public async Task<ActionResult> RemovePlaylistRef ([FromQuery] Guid playlistId, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new RemovePlaylistRefCommand(GetUserId(), playlistId), cancellationToken));
        }

        [HttpGet("Ref"), Authorize(Policy = "user")]
        public async Task<ActionResult<GetPlaylistRefResponseDto?>> GetPlaylistRef (Guid playlistId, CancellationToken cancellationToken = default) {
            var playlistRef = await _mediator.Send(new GetPlaylistRefQuery(GetUserId(), playlistId), cancellationToken);

            if (playlistRef != null) {
                return new GetPlaylistRefResponseDto {
                    Exists = true,
                    CreateDate = playlistRef.CreateDate
                };
            } else {
                return new GetPlaylistRefResponseDto {
                    Exists = false
                };
            }
        }

        [HttpPost("Ref"), Authorize(Policy = "user")]
        public async Task<IActionResult> CreatePlaylistRef (AddPlaylistToLibraryRequestDto request, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new CreatePlaylistRefCommand(GetUserId(), request.PlaylistId), cancellationToken));
        }

        private string? GetOptionalUserId () {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        }

        private string GetUserId () {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                throw new AppException("Unauthorized", null, StatusCodes.Status403Forbidden);
            }
            return userId;
        }

    }
}
