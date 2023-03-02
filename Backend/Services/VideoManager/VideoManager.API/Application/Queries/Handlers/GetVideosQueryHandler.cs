using Application.Handlers;
using AutoMapper;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.Utilities;
using VideoManager.Domain.Contracts;

namespace VideoManager.API.Application.Queries.Handlers {
    public class GetVideosQueryHandler : IQueryHandler<GetVideosQuery, GetVideosResponseDto> {

        private readonly IVideoRepository _repository;
        private readonly IMapper _mapper;

        public GetVideosQueryHandler (IVideoRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetVideosResponseDto> Handle (GetVideosQuery request, CancellationToken cancellationToken) {
            var videos = await _repository.GetVideosByUserIdAsync(
                request.UserId,
                request.Request.Page,
                request.Request.PageSize,
                request.Request.Sort.ToVideoSort());

            var totalCount = await _repository.GetVideosCountByUserIdAsync(request.UserId);

            return new GetVideosResponseDto {
                Videos = _mapper.Map<List<VideoDto>>(videos, options => options.Items["resolveUrl"] = true),
                TotalCount = totalCount
            };

        }

    }
}
