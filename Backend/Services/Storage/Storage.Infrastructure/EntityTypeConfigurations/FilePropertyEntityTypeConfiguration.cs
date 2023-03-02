using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Domain.Models;

namespace Storage.Infrastructure.EntityTypeConfigurations {
    public class FilePropertyEntityTypeConfiguration : IEntityTypeConfiguration<FileProperty> {

        public void Configure (EntityTypeBuilder<FileProperty> builder) {
            builder.Property<int>("Id").UseHiLo("file_property_seq");
            builder.HasKey("Id");

            builder.Property(x => x.Name).IsRequired();

            builder.Property(x => x.Value).IsRequired();
        }

    }
}
