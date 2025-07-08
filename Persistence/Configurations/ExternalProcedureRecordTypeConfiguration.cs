using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class ExternalProcedureRecordTypeConfiguration : IEntityTypeConfiguration<ExternalProcedureRecordType>
    {
        public void Configure(EntityTypeBuilder<ExternalProcedureRecordType> builder)
        {
            builder.ToTable(TableNames.ExternalProcedureRecordType);

            builder.HasKey(u => u.Id);
        }
    }
}
