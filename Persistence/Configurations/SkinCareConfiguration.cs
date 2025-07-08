using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class SkinCareConfiguration : IEntityTypeConfiguration<SkinCare>
    {
        public void Configure(EntityTypeBuilder<SkinCare> builder)
        {
            builder.ToTable(TableNames.SkinCare);

            builder.HasKey(e => e.Id);
            builder
                .HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeId);
        }
    }
}
