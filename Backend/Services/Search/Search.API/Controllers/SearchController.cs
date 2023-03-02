using MediatR;
using Microsoft.AspNetCore.Mvc;
using Search.API.Application.DtoModels;
using Search.API.Application.Queries;
using Search.Infrastructure.Specifications;

namespace Search.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class SearchController : ControllerBase {

        private readonly IMediator _mediator;

        public SearchController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet("Tags/Relevant")]
        public async Task<List<string>> SearchRelevantTags (string tags = "", CancellationToken cancellationToken = default) {
            return await _mediator.Send(new SearchRelevantTagsQuery(tags, 20), cancellationToken);
        }

        [HttpGet("Tags/Trending")]
        public async Task<List<string>> SearchTrendingTags (CancellationToken cancellationToken) {
            return await _mediator.Send(new SearchTrendingTagsQuery(20), cancellationToken);
        }

        [HttpGet]
        public async Task<SearchResponseDto> SearchByQuery ([FromQuery] SearchByQueryRequestDto request, CancellationToken cancellationToken = default) {
            var searchParams = new VideoSearchParameters {
                Query = request.Query,
                Pagination = new Pagination(request.Page ?? 1, request.PageSize ?? 20),
                Sort = (VideoSort)(request.Sort ?? SearchSort.Relevance),
                PeriodRange = new PeriodRange(request.From, request.To)
            };

            return await _mediator.Send(new SearchVideosQuery(searchParams), cancellationToken);
        }

        [HttpPost("ByTags")]
        public async Task<SearchResponseDto> SearchByTags (SearchByTagsRequestDto request, CancellationToken cancellationToken) {
            var tagsList = request.Tags.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            return await _mediator.Send(new SearchVideosByTagsQuery(tagsList, new Pagination(request.Page ?? 1, request.PageSize ?? 20)), cancellationToken);
        }

        [HttpPost("ByCreators")]
        public async Task<SearchResponseDto> SearchByCreatorIds (SearchByCreatorIdsDto request, CancellationToken cancellationToken) {
            var searchParams = new VideoSearchParameters {
                CreatorIds = request.CreatorIds.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct(),
                Pagination = new Pagination(request.Page ?? 1, request.PageSize ?? 20),
                Sort = VideoSort.CreateDate,
            };

            return await _mediator.Send(new SearchVideosQuery(searchParams), cancellationToken);
        }

    }
}
