using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Infrastructure.EntityTypeConfigurations {
    public class VideoProcessingStepEntityTypeConfiguration : IEntityTypeConfiguration<VideoProcessingStep> {

        public void Configure (EntityTypeBuilder<VideoProcessingStep> builder) {
            builder.Property<int>("Id");
            builder.HasKey("Id");

            builder.Property(x => x.Label).IsRequired();

            builder.Property(x => x.Height).IsRequired();

            builder.Property(x => x.Complete).IsRequired();
        }

    }
}
