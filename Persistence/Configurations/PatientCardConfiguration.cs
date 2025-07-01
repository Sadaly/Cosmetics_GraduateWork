using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class PatientCardConfiguration : IEntityTypeConfiguration<PatientCard>
    {
        public void Configure(EntityTypeBuilder<PatientCard> builder)
        {
            builder.ToTable(TableNames.PatientCard);

            builder.HasKey(pc => pc.Id);
        }
    }
}
