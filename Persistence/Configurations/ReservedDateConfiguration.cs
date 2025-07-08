using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class ReservedDateConfiguration : IEntityTypeConfiguration<ReservedDate>
    {
        public void Configure(EntityTypeBuilder<ReservedDate> builder)
        {
            builder.ToTable(TableNames.ReservedDate);

            builder.HasKey(u => u.Id);
        }
    }
}
