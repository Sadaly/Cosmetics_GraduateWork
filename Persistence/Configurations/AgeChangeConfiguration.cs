using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class AgeChangeConfiguration : IEntityTypeConfiguration<AgeChange>
	{
		public void Configure(EntityTypeBuilder<AgeChange> builder)
		{
			builder.ToTable(TableNames.AgeChange);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Type)
				.WithMany()
				.HasForeignKey(e => e.TypeId);
		}
	}
}
