using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoStore.Domain.Models;

namespace VideoStore.Infrastructure.EntityTypeConfigurations {
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<UserProfile> {

        public void Configure (EntityTypeBuilder<UserProfile> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.PrimaryVersion).IsConcurrencyToken();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName).IsRequired();

            builder.Property(x => x.Handle).IsRequired(false);

            builder.Property(x => x.ThumbnailUrl).IsRequired(false);

            var videosNavigation = builder.Metadata.FindNavigation(nameof(UserProfile.Videos))!;
            videosNavigation.SetField("_videos");
            videosNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Videos)
                .WithOne(x => x.CreatorProfile)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
