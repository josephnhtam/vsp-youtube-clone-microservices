using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Infrastructure.EntityTypeConfigurations {
    public class ProcessedVideoEntityTypeConfiguration : IEntityTypeConfiguration<ProcessedVideo> {

        public void Configure (EntityTypeBuilder<ProcessedVideo> builder) {
            builder.Property<int>("Id");
            builder.HasKey("Id");

            builder.Property(x => x.VideoFileId).IsRequired();

            builder.Property(x => x.Label).IsRequired();

            builder.Property(x => x.Width).IsRequired();

            builder.Property(x => x.Height).IsRequired();

            builder.Property(x => x.Size).IsRequired();

            builder.Property(x => x.LengthSeconds).IsRequired();

            builder.Property(x => x.Url).IsRequired();
        }

    }
}
