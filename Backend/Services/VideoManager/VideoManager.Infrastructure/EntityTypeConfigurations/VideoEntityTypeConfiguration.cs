using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoManager.Domain.Models;

namespace VideoManager.Infrastructure.EntityTypeConfigurations {
    public class VideoEntityTypeConfiguration : IEntityTypeConfiguration<Video> {

        public void Configure (EntityTypeBuilder<Video> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.InfoVersion).IsConcurrencyToken();
            builder.Property(x => x.PublishStatusVersion).IsConcurrencyToken();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatorId).IsRequired();

            builder.HasIndex(x => x.CreatorId);

            builder.Property(x => x.Title).IsRequired();

            builder.Property(x => x.Description).IsRequired();

            builder.Property(x => x.Tags).IsRequired();

            builder.Property(x => x.ThumbnailId).IsRequired(false);

            builder.Property(x => x.Status).IsRequired();

            builder.HasIndex(x => x.Status);

            builder.Property(x => x.Visibility).IsRequired();

            builder.HasIndex(x => x.Visibility);

            builder.Property(x => x.AllowedToPublish).IsRequired();

            builder.HasIndex(x => x.AllowedToPublish);

            builder.Property(x => x.ProcessingStatus).IsRequired();

            builder.HasIndex(x => x.ProcessingStatus);

            builder.Property(x => x.OriginalVideoFileId).IsRequired();

            builder.HasIndex(x => x.OriginalVideoFileId);

            builder.Property(x => x.OriginalVideoFileName).IsRequired(false);

            builder.Property(x => x.OriginalVideoUrl).IsRequired(false);

            var videosNavigation = builder.Metadata.FindNavigation(nameof(Video.Videos))!;
            videosNavigation.SetField("_videos");
            videosNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Videos).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ThumbnailStatus).IsRequired();

            builder.HasIndex(x => x.ThumbnailStatus);

            var thumbnailsNavigation = builder.Metadata.FindNavigation(nameof(Video.Thumbnails))!;
            thumbnailsNavigation.SetField("_thumbnails");
            thumbnailsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Thumbnails).WithOne().OnDelete(DeleteBehavior.Cascade);

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

            builder.Property(x => x.CreateDate).IsRequired();

            builder.HasIndex(x => x.CreateDate);

            builder.Property(x => x.PublishDate).IsRequired(false);

            builder.HasIndex(x => x.PublishDate);

            builder.Property(x => x.UnpublishDate).IsRequired(false);

            var videoMetricsBuilder = builder.OwnsOne(x => x.Metrics);
            videoMetricsBuilder.Property(x => x.ViewsCount)
                .HasColumnName("ViewsCount")
                .IsRequired();

            videoMetricsBuilder.Property(x => x.CommentsCount)
                .HasColumnName("CommentsCount")
                .IsRequired();

            videoMetricsBuilder.Property(x => x.LikesCount)
                .HasColumnName("LikesCount")
                .IsRequired();

            videoMetricsBuilder.Property(x => x.DislikesCount)
                .HasColumnName("DislikesCount")
                .IsRequired();

            videoMetricsBuilder.Property(x => x.ViewsCountUpdateDate)
                .HasColumnName("ViewsCountUpdateDate")
                .IsRequired(false);

            videoMetricsBuilder.Property(x => x.CommentsCountUpdateDate)
                .HasColumnName("CommentsCountUpdateDate")
                .IsRequired(false);

            videoMetricsBuilder.Property(x => x.VotesCountUpdateDate)
                .HasColumnName("VotesCountUpdateDate")
                .IsRequired(false);

            videoMetricsBuilder.HasIndex(x => x.ViewsCount);
            videoMetricsBuilder.HasIndex(x => x.CommentsCount);
            videoMetricsBuilder.HasIndex(x => x.LikesCount);
            videoMetricsBuilder.HasIndex(x => x.DislikesCount);
        }

    }
}
