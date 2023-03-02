using Library.API.Application.DtoModels;
using Library.API.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class LibraryController : ControllerBase {

        private readonly IMediator _mediator;

        public LibraryController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<GetLibraryResourcesResponseDto>> GetLibraryResources (
            GetLibraryResourcesRequestDto request, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(
                new GetLibraryResourcesQuery(
                    request.TargetUserId,
                    request.RequireUploadedVideos,
                    request.RequireCreatedPlaylistInfos,
                    request.RequireVideos,
                    request.RequirePlaylists,
                    request.RequirePlaylistInfos,
                    request.MaxUploadedVideosCount,
                    request.MaxCreatedPlaylistsCount,
                    request.MaxPlaylistItemsCount
                ),
                cancellationToken));
        }

    }
}
