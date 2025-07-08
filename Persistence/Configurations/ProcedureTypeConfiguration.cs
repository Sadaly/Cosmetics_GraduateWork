using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class ProcedureTypeConfiguration : IEntityTypeConfiguration<ProcedureType>
    {
        public void Configure(EntityTypeBuilder<ProcedureType> builder)
        {
            builder.ToTable(TableNames.ProcedureType);

            builder.HasKey(u => u.Id);
        }
    }
}
