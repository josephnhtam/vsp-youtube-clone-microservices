using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VideoStore.API.Application.DtoModels;
using VideoStore.API.Application.Queries;

namespace VideoStore.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class VideoStoreController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public VideoStoreController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("Videos/{id}")]
        public async Task<ActionResult<VideoDto>> GetVideo (Guid id, CancellationToken cancellationToken) {
            var video = await _mediator.Send(new GetVideoQuery(GetOptionalUserId(), id), cancellationToken);
            return Ok(_mapper.Map<VideoDto>(video, options => options.Items["resolveUrl"] = true));
        }

        private string? GetOptionalUserId () {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        }

    }
}
