using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Domain.Models;

namespace Storage.Infrastructure.EntityTypeConfigurations {
    public class StoredFileEntityTypeConfiguration : IEntityTypeConfiguration<StoredFile> {

        public void Configure (EntityTypeBuilder<StoredFile> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.HasKey(x => x.FileId);
            builder.Property(x => x.FileId);

            builder.Property(x => x.TrackingId).IsRequired();

            builder.HasIndex(x => x.TrackingId).IsUnique();

            builder.Property(x => x.GroupId).IsRequired();

            builder.HasIndex(x => x.GroupId);

            builder.Property(x => x.OriginalFileName).IsRequired();

            builder.Property(x => x.FileName).IsRequired();

            builder.Property(x => x.Category).IsRequired();

            builder.HasIndex(x => x.Category);

            builder.Property(x => x.FileSize).IsRequired();

            builder.HasIndex(x => x.FileSize);

            builder.Property(x => x.Url).IsRequired();

            builder.Property(x => x.UserId).IsRequired(false);

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.ContentType).IsRequired(false);

            builder.HasIndex(x => x.ContentType);

            builder.Property(x => x.CreateDate).IsRequired();

            builder.HasIndex(x => x.CreateDate);

            builder.HasMany(x => x.Properties).WithOne().OnDelete(DeleteBehavior.Cascade);

            var propertiesNavigation = builder.Metadata.FindNavigation(nameof(StoredFile.Properties))!;
            propertiesNavigation.SetField("_properties");
            propertiesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
