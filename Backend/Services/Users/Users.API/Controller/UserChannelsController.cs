using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Users.API.Application.DtoModels;
using Users.API.Application.Queries;

namespace Users.API.Controller {
    [ApiController, Route("api/v1/[controller]")]
    public class UserChannelsController : ControllerBase {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UserChannelsController (IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("Info")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DetailedUserChannelInfoDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Channel not found")]
        public async Task<ActionResult<DetailedUserChannelInfoDto>> GetDetailedUserChannelInfo (string? userId = null, string? handle = null, CancellationToken cancellationToken = default) {
            var getUserProfileTask = _mediator.Send(new GetUserProfileQuery(userId, handle), cancellationToken);
            var getUserChannelTask = _mediator.Send(new GetUserChannelQuery(userId, handle, 0), cancellationToken);

            await Task.WhenAll(getUserChannelTask, getUserProfileTask);

            var userProfile = getUserProfileTask.Result;
            var userChannel = getUserChannelTask.Result;

            return Ok(new DetailedUserChannelInfoDto {
                UserProfile = _mapper.Map<DetailedUserProfileDto>(userProfile, options => options.Items["resolveUrl"] = true),
                UserChannelInfo = _mapper.Map<UserChannelInfoDto>(userChannel, options => options.Items["resolveUrl"] = true)
            });
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserChannelDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Channel not found")]
        public async Task<ActionResult<UserChannelDto>> GetChannel (string? userId = null, string? handle = null, int? maxSectionItemsCount = 12, int? sectionType = null, CancellationToken cancellationToken = default) {
            var userChannel = await _mediator.Send(new GetUserChannelQuery(userId, handle, maxSectionItemsCount), cancellationToken);
            var channelDto = _mapper.Map<UserChannelDto>(userChannel, options => options.Items["resolveUrl"] = true);

            if (sectionType != null) {
                channelDto.Sections = channelDto.Sections.Where(x => (x.Type & (ChannelSectionType)sectionType) != 0).ToList();
            }

            return Ok(channelDto);
        }

        [HttpGet("{userId}/Section/{sectionId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ChannelSectionDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Channel section not found")]
        public async Task<ActionResult<ChannelSectionDto>> GetChannelSection (string userId, Guid sectionId, CancellationToken cancellationToken = default) {
            var channelSection = await _mediator.Send(new GetChannelSectionQuery(userId, sectionId), cancellationToken);
            return Ok(_mapper.Map<ChannelSectionDto>(channelSection));
        }

    }
}
