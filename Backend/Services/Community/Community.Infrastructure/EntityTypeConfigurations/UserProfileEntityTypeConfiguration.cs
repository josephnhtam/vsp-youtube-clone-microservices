using Community.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Community.Infrastructure.EntityTypeConfigurations {
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<UserProfile> {

        public void Configure (EntityTypeBuilder<UserProfile> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.PrimaryVersion).IsConcurrencyToken();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName).IsRequired();

            builder.Property(x => x.Handle).IsRequired(false);

            builder.Property(x => x.ThumbnailUrl).IsRequired(false);

            builder.HasMany(x => x.VideoForums)
                .WithOne(x => x.CreatorProfile)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.VideoCommentVotes)
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var videoForumsNavigation = builder.Metadata.FindNavigation(nameof(UserProfile.VideoForums))!;
            videoForumsNavigation.SetField("_videoForums");
            videoForumsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var videoCommentVotesNavigation = builder.Metadata.FindNavigation(nameof(UserProfile.VideoCommentVotes))!;
            videoCommentVotesNavigation.SetField("_videoCommentVotes");
            videoCommentVotesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
