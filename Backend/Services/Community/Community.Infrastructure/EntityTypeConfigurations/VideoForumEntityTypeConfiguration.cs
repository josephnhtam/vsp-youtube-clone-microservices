using Community.Domain.Models;
using Infrastructure.EFCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Community.Infrastructure.EntityTypeConfigurations {
    public class VideoForumEntityTypeConfiguration : IEntityTypeConfiguration<VideoForum> {

        public void Configure (EntityTypeBuilder<VideoForum> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.HasKey(x => x.VideoId);

            builder.Property(x => x.CreatorId).IsRequired();

            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.AllowedToComment).IsRequired();

            builder.Property(x => x.VideoCommentsCount)
                .IsRequired()
               .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());

            builder.Property(x => x.RootVideoCommentsCount)
                .IsRequired()
              .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());

            builder.HasIndex(x => x.VideoCommentsCount);

            builder.HasMany(x => x.VideoComments)
                .WithOne(x => x.VideoForum)
                .HasForeignKey(x => x.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.VideoCommentVotes)
               .WithOne()
               .HasForeignKey(x => x.VideoId)
               .OnDelete(DeleteBehavior.Cascade);

            var videoForumsNavigation = builder.Metadata.FindNavigation(nameof(VideoForum.VideoComments))!;
            videoForumsNavigation.SetField("_videoComments");
            videoForumsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var videoCommentVotesNavigation = builder.Metadata.FindNavigation(nameof(VideoForum.VideoCommentVotes))!;
            videoCommentVotesNavigation.SetField("_videoCommentVotes");
            videoCommentVotesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
