using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subscriptions.Domain.Models;

namespace Subscriptions.Infrastructure.EntityTypeConfigurations {
    public class SubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<Subscription> {

        public void Configure (EntityTypeBuilder<Subscription> builder) {

            builder.HasKey(x => new { x.UserId, x.TargetId });

            builder.Property(x => x.NotificationType).IsRequired();

            builder.Property(x => x.SubscriptionDate).IsRequired();

            builder.HasOne(x => x.Target)
                .WithMany()
                .HasForeignKey(x => x.TargetId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
