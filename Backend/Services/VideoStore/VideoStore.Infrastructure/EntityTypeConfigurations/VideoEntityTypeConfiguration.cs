using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoStore.Domain.Models;

namespace VideoStore.Infrastructure.EntityTypeConfigurations {
    public class VideoEntityTypeConfiguration : IEntityTypeConfiguration<Video> {

        public void Configure (EntityTypeBuilder<Video> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.InfoVersion).IsConcurrencyToken();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatorId).IsRequired();

            builder.HasIndex(x => x.CreatorId);

            builder.Property(x => x.Title).IsRequired();

            builder.Property(x => x.Description).IsRequired();

            builder.Property(x => x.Tags).IsRequired();

            builder.Property(x => x.ThumbnailUrl).IsRequired(false);

            builder.Property(x => x.PreviewThumbnailUrl).IsRequired(false);

            builder.Property(x => x.Visibility).IsRequired();

            builder.HasIndex(x => x.Visibility);

            builder.Property(x => x.AllowedToPublish).IsRequired();

            builder.Property(x => x.PublishedWithPublicVisibility).IsRequired();

            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.CreateDate).IsRequired();

            builder.HasIndex(x => x.CreateDate);

            builder.Property(x => x.StatusUpdateDate).IsRequired(false);

            var videosNavigation = builder.Metadata.FindNavigation(nameof(Video.Videos))!;
            videosNavigation.SetField("_videos");
            videosNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Videos).WithOne().OnDelete(DeleteBehavior.Cascade);

            var videoMetricsBuilder = builder.OwnsOne(x => x.Metrics);
            videoMetricsBuilder.Property(x => x.ViewsCount)
                .HasColumnName("ViewsCount")
                .IsRequired();

            videoMetricsBuilder.Property(x => x.ViewsCountUpdateDate)
                .HasColumnName("ViewsCountUpdateDate")
                .IsRequired(false);
        }

    }
}
