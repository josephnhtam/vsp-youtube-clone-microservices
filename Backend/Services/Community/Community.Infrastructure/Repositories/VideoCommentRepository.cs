using Community.Domain.Contracts;
using Community.Domain.Models;
using Community.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.Repositories {
    public class VideoCommentRepository : IVideoCommentRepository {

        private readonly CommunityDbContext _dbContext;

        public VideoCommentRepository (CommunityDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddVideoCommentAsync (VideoComment videoComment) {
            await _dbContext.AddAsync(videoComment);
        }


        public Task DeleteVideoCommentAsync (VideoComment videoComment) {
            _dbContext.Remove(videoComment);
            return Task.CompletedTask;
        }

        public Task<VideoComment?> GetVideoCommentByIdAsync (long id, bool includeVideoForum = true, bool includeUserProfile = false) {
            var videoComment = _dbContext.VideoComments.Where(x => x.Id == id);

            int includeCount = 0;

            if (includeVideoForum) {
                videoComment = videoComment.Include(x => x.VideoForum);
                includeCount++;
            }

            if (includeUserProfile) {
                videoComment = videoComment.Include(x => x.UserProfile);
                includeCount++;
            }

            if (includeCount > 1) {
                videoComment = videoComment.AsSplitQuery();
            }

            return videoComment.FirstOrDefaultAsync();
        }

        public Task<List<VideoComment>> GetRootVideoCommentsAsync (Guid videoId, DateTimeOffset? maxDate, int page, int pageSize, VideoCommentSort sort) {
            var query = _dbContext.VideoComments
                .Include(x => x.UserProfile)
                .Where(x => x.VideoId == videoId && x.ParentCommentId == null);

            query = SortVideoComments(query, sort);

            if (maxDate != null) {
                query = query.Where(x => x.CreateDate < maxDate);
            }

            query = query
                .Skip(Math.Max(0, (page - 1) * pageSize))
                .Take(pageSize);

            return query.ToListAsync();
        }

        public Task<List<VideoComment>> GetUserRootVideoCommentsAsync (Guid videoId, string userId, int maxCount) {
            var query = _dbContext.VideoComments
             .Include(x => x.UserProfile)
             .Where(x => x.UserId == userId)
             .Where(x => x.VideoId == videoId && x.ParentCommentId == null)
             .OrderByDescending(x => x.CreateDate)
             .Take(maxCount);

            return query.ToListAsync();
        }

        private static IQueryable<VideoComment> SortVideoComments (IQueryable<VideoComment> query, VideoCommentSort sort) {
            switch (sort) {
                default:
                case VideoCommentSort.Date:
                    query = query.OrderByDescending(x => x.CreateDate);
                    break;
                case VideoCommentSort.LikesCount:
                    query = query.OrderByDescending(x => x.LikesCount).ThenByDescending(x => x.CreateDate);
                    break;
                case VideoCommentSort.RepliesCount:
                    query = query.OrderByDescending(x => x.RepliesCount).ThenByDescending(x => x.CreateDate);
                    break;
            }

            return query;
        }

        public Task<List<VideoComment>> GetVideoCommentRepliesIdAsync (long id, int page, int pageSize) {
            return _dbContext.VideoComments
                .Include(x => x.UserProfile)
                .Where(x => x.ParentCommentId == id)
                .OrderBy(x => x.CreateDate)
                .Skip(Math.Max(0, (page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
