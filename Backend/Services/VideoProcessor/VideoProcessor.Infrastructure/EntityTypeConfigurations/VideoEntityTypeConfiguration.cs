using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Infrastructure.EntityTypeConfigurations {
    public class VideoEntityTypeConfiguration : IEntityTypeConfiguration<Video> {

        public void Configure (EntityTypeBuilder<Video> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.LockVersion)
                .IsConcurrencyToken()
                .IsRequired();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatorId).IsRequired();

            builder.HasIndex(x => x.CreatorId);

            builder.Property(x => x.OriginalFileName).IsRequired();

            builder.Property(x => x.VideoFileUrl).IsRequired();

            builder.Property(x => x.Status).IsRequired();

            builder.HasIndex(x => x.Status);

            builder.Property(x => x.RetryCount).IsRequired();

            builder.Property(x => x.AvailableDate).IsRequired();

            builder.HasIndex(x => x.AvailableDate);

            builder.Property(x => x.ProcessedDate).IsRequired(false);

            builder.HasIndex(x => x.ProcessedDate);

            var processingStepsNavigation = builder.Metadata.FindNavigation(nameof(Video.ProcessingSteps))!;
            processingStepsNavigation.SetField("_processingSteps");
            processingStepsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.ProcessingSteps).WithOne().OnDelete(DeleteBehavior.Cascade);

            var thumbnailsNavigation = builder.Metadata.FindNavigation(nameof(Video.Thumbnails))!;
            thumbnailsNavigation.SetField("_thumbnails");
            thumbnailsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Thumbnails).WithOne().OnDelete(DeleteBehavior.Cascade);

            var videosNavigation = builder.Metadata.FindNavigation(nameof(Video.Videos))!;
            videosNavigation.SetField("_videos");
            videosNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Videos).WithOne().OnDelete(DeleteBehavior.Cascade);

            var previewThumbnailBuilder = builder.OwnsOne(x => x.PreviewThumbnail);

            previewThumbnailBuilder.Property(x => x.ImageFileId)
                .HasColumnName("PreviewThumbnailImageFileId")
                .IsRequired(true);

            previewThumbnailBuilder.Property(x => x.Width)
                .HasColumnName("PreviewThumbnailWidth")
                .IsRequired(true);

            previewThumbnailBuilder.Property(x => x.Height)
                .HasColumnName("PreviewThumbnailHeight")
                .IsRequired(true);

            previewThumbnailBuilder.Property(x => x.LengthSeconds)
                .HasColumnName("PreviewThumbnailLengthSeconds")
                .IsRequired(true);

            previewThumbnailBuilder.Property(x => x.Url)
               .HasColumnName("PreviewThumbnailLengthUrl")
               .IsRequired(true);

            var videoInfoBuilder = builder.OwnsOne(x => x.VideoInfo);

            videoInfoBuilder.Property(x => x.Width).IsRequired(true);

            videoInfoBuilder.Property(x => x.Height).IsRequired(true);

            videoInfoBuilder.Property(x => x.Size).IsRequired(true);

            videoInfoBuilder.Property(x => x.LengthSeconds).IsRequired(true);
        }

    }
}
