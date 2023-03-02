using Infrastructure.EFCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subscriptions.Domain.Models;

namespace Subscriptions.Infrastructure.EntityTypeConfigurations {
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<UserProfile> {

        public void Configure (EntityTypeBuilder<UserProfile> builder) {
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.PrimaryVersion).IsConcurrencyToken();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName).IsRequired();

            builder.Property(x => x.Description).IsRequired();

            builder.Property(x => x.Handle).IsRequired(false);

            builder.HasIndex(x => x.Handle);

            builder.Property(x => x.ThumbnailUrl).IsRequired(false);

            builder.Property(x => x.SubscribersCount)
                .IsRequired()
                .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<long>());

            builder.Property(x => x.SubscriptionsCount)
                .IsRequired()
                .Metadata.SetValueComparer(AlwaysTrueValueComparer.Create<int>());
        }

    }
}
