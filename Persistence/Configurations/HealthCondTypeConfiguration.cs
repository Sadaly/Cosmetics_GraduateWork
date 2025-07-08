using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class HealthCondTypeConfiguration : IEntityTypeConfiguration<HealthCondType>
    {
        public void Configure(EntityTypeBuilder<HealthCondType> builder)
        {
            builder.ToTable(TableNames.HealthCondType);

            builder.HasKey(u => u.Id);
        }
    }
}
