using Infrastructure.EFCore.TransactionalEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore.TransactionalEvents.Extensions {
    public static class ModelBuilderExtensions {

        public static void AddTransactionalEventsModels (this ModelBuilder modelBuilder) {
            AddTransactionalEventsGroup(modelBuilder);
            AddTransactionalEvent(modelBuilder);
        }

        private static void AddTransactionalEventsGroup (ModelBuilder modelBuilder) {
            var builder = modelBuilder.Entity<TransactionalEventsGroup>().ToTable("_TransactionalEventsGroup");

            builder.Property(x => x.Id).IsRequired();
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreateDate).IsRequired();
            builder.HasIndex(x => x.CreateDate);

            builder.Property(x => x.AvailableDate).IsRequired();
            builder.HasIndex(x => x.AvailableDate);

            builder.Property(x => x.LastSequenceNumber).IsRequired();
            builder.HasIndex(x => x.LastSequenceNumber);

            builder.HasMany(x => x.TransactionalEvents)
                .WithOne()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void AddTransactionalEvent (ModelBuilder modelBuilder) {
            var builder = modelBuilder.Entity<TransactionalEventData>().ToTable("_TransactionalEvents");

            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.HasKey(x => x.Id);

            builder.Property(x => x.GroupId).IsRequired();
            builder.HasIndex(x => x.GroupId);

            builder.Property(x => x.SequenceNumber).IsRequired();
            builder.HasIndex(x => x.SequenceNumber);

            var eventBuilder = builder.OwnsOne(x => x.Event);

            eventBuilder.Property(x => x.Category).HasColumnName("Category").IsRequired();
            eventBuilder.HasIndex(x => x.Category);

            eventBuilder.Property(x => x.Type).HasColumnName("Type").IsRequired();
            eventBuilder.Property(x => x.Data).HasColumnName("Data").IsRequired();
        }

    }
}
