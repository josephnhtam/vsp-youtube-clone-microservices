using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Domain.Models;

namespace Storage.Infrastructure.EntityTypeConfigurations {
    public class FileTrackingEntityTypeConfiguration : IEntityTypeConfiguration<FileTracking> {

        public void Configure (EntityTypeBuilder<FileTracking> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();

            builder.HasKey(x => x.TrackingId);
            builder.Property(x => x.TrackingId);

            builder.Property(x => x.GroupId).IsRequired();

            builder.HasIndex(x => x.GroupId);

            builder.Property(x => x.FileId).IsRequired();

            builder.HasIndex(x => x.FileId);

            builder.Property(x => x.Category).IsRequired();

            builder.Property(x => x.ContentType).IsRequired(false);

            builder.Property(x => x.FileName).IsRequired();

            builder.Property(x => x.OriginalFileName).IsRequired();

            builder.Property(x => x.Status).IsRequired();

            builder.HasIndex(x => x.Status);

            builder.Property(x => x.RemovalRetryCount).IsRequired();

            builder.Property(x => x.CreateDate).IsRequired();

            builder.HasIndex(x => x.CreateDate);

            builder.Property(x => x.RemovalDate).IsRequired(false);

            builder.HasIndex(x => x.RemovalDate);
        }

    }
}
