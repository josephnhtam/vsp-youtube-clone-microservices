using Community.Domain.Models;
using Infrastructure.EFCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Community.Infrastructure.EntityTypeConfigurations {
    public class VideoCommentEntityTypeConfiguration : IEntityTypeConfiguration<VideoComment> {

        public void Configure (EntityTypeBuilder<VideoComment> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VideoId).IsRequired();

            builder.Property(x => x.ParentCommentId).IsRequired(false);

            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => x.VideoId);

            builder.HasIndex(nameof(VideoComment.VideoId), nameof(VideoComment.ParentCommentId));

            builder.Property(x => x.Comment).IsRequired();

            builder.Property(x => x.LikesCount)
                .IsRequired()
                .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());

            builder.HasIndex(x => x.LikesCount);

            builder.Property(x => x.DislikesCount)
                .IsRequired()
                .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());

            builder.Property(x => x.RepliesCount)
                .IsRequired()
                .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());

            builder.HasIndex(x => x.RepliesCount);

            builder.Property(x => x.CreateDate)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
              .IsRequired();

            builder.HasIndex(x => x.CreateDate);

            builder.Property(x => x.EditDate).IsRequired(false);

            builder.HasOne(x => x.UserProfile)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Replies)
                .WithOne(x => x.ParentComment)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.VideoCommentVotes)
                .WithOne()
                .HasForeignKey(x => x.VideoCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            var videoForumsNavigation = builder.Metadata.FindNavigation(nameof(VideoComment.Replies))!;
            videoForumsNavigation.SetField("_replies");
            videoForumsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var videoCommentVotesNavigation = builder.Metadata.FindNavigation(nameof(VideoComment.VideoCommentVotes))!;
            videoCommentVotesNavigation.SetField("_videoCommentVotes");
            videoCommentVotesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
