using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class HealthCondConfiguration : IEntityTypeConfiguration<HealthCond>
	{
		public void Configure(EntityTypeBuilder<HealthCond> builder)
		{
			builder.ToTable(TableNames.HealthCond);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Type)
				.WithMany()
				.HasForeignKey(e => e.TypeId);
		}
	}
}
