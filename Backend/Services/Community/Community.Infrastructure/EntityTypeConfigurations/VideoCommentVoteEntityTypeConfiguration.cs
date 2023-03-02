using Community.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Community.Infrastructure.EntityTypeConfigurations {
    public class VideoCommentVoteEntityTypeConfiguration : IEntityTypeConfiguration<VideoCommentVote> {

        public void Configure (EntityTypeBuilder<VideoCommentVote> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();

            builder.HasKey(nameof(VideoCommentVote.UserId), nameof(VideoCommentVote.VideoCommentId));

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.VideoCommentId).IsRequired();

            builder.Property(x => x.VideoId).IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.VideoCommentId);

            builder.HasIndex(x => x.VideoId);

            builder.HasIndex(nameof(VideoCommentVote.UserId), nameof(VideoCommentVote.VideoId));

            builder.Property(x => x.Type);
        }

    }
}
