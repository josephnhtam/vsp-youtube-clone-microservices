using Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore.Idempotency.Extensions {
    public static class ModelBuilderExtensions {

        public static void AddIdempotentOperationModel (this ModelBuilder modelBuilder) {
            var builder = modelBuilder.Entity<IdempotentOperation>().ToTable("_IdempotentOperation");

            builder.Property(x => x.Id).IsRequired();
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date).IsRequired();
        }

    }
}
