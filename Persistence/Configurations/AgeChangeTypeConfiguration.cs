using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class AgeChangeTypeConfiguration : IEntityTypeConfiguration<AgeChangeType>
    {
        public void Configure(EntityTypeBuilder<AgeChangeType> builder)
        {
            builder.ToTable(TableNames.AgeChangeType);

            builder.HasKey(u => u.Id);
        }
    }
}
