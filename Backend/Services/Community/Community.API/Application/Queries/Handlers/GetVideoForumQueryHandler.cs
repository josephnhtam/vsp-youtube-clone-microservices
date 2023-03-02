using Application.Handlers;
using AutoMapper;
using Community.API.Application.DtoModels;
using Community.Domain.Contracts;
using Community.Domain.Models;
using SharedKernel.Exceptions;

namespace Community.API.Application.Queries.Handlers {
    public class GetVideoForumQueryHandler : IQueryHandler<GetVideoForumQuery, GetVideoForumResponseDto> {

        private readonly IVideoForumRepository _forumRepository;
        private readonly IVideoCommentVoteRepository _commentVoteRepository;
        private readonly IVideoCommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private const int PinnedUserCommentCount = 2;

        public GetVideoForumQueryHandler (IVideoForumRepository forumRepository, IVideoCommentVoteRepository commentVoteRepository, IVideoCommentRepository commentRepository, IMapper mapper) {
            _forumRepository = forumRepository;
            _commentVoteRepository = commentVoteRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<GetVideoForumResponseDto> Handle (GetVideoForumQuery request, CancellationToken cancellationToken) {
            var videoForum = await _forumRepository.GetVideoForumByIdAsync(request.VideoId);

            if (videoForum == null) {
                throw new AppException("Forum not found", null, StatusCodes.Status404NotFound);
            }

            List<VideoComment>? userComments = null;
            List<UserVideoCommentVote> votes;

            if (request.UserId != null) {
                userComments = await _commentRepository.GetUserRootVideoCommentsAsync(request.VideoId, request.UserId, PinnedUserCommentCount);
                votes = await _commentVoteRepository.GetVotedVideoCommentsAsync(request.VideoId, request.UserId);
            } else {
                votes = new List<UserVideoCommentVote>();
            }

            var currentTime = DateTimeOffset.UtcNow;

            var comments = await _commentRepository.GetRootVideoCommentsAsync(request.VideoId, currentTime, 1, request.PageSize, request.Sort);

            if (userComments != null) {
                comments.RemoveAll(x => userComments.Any(y => y.Id == x.Id));
            }

            return new GetVideoForumResponseDto {
                CommentsCount = videoForum.VideoCommentsCount,
                RootCommentsCount = videoForum.RootVideoCommentsCount,
                LikedCommentIds = votes.Where(x => x.VoteType == VoteType.Like).Select(x => x.CommentId),
                DislikedCommentIds = votes.Where(x => x.VoteType == VoteType.Dislike).Select(x => x.CommentId),
                Comments = _mapper.Map<List<VideoCommentDto>>(comments, options => options.Items["resolveUrl"] = true),
                PinnedUserComments = userComments == null ? null : _mapper.Map<List<VideoCommentDto>>(userComments, options => options.Items["resolveUrl"] = true),
                LoadTime = currentTime
            };
        }

    }
}
