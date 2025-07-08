using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class ExternalProcedureRecordConfiguration : IEntityTypeConfiguration<ExternalProcedureRecord>
    {
        public void Configure(EntityTypeBuilder<ExternalProcedureRecord> builder)
        {
            builder.ToTable(TableNames.ExternalProcedureRecord);

            builder.HasKey(e => e.Id);
            builder
                .HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeId);
        }
    }
}
