using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ProcedureResourceConfiguration : IEntityTypeConfiguration<ProcedureResource>
	{
		public void Configure(EntityTypeBuilder<ProcedureResource> builder)
		{
			builder.ToTable(TableNames.ProcedureResource);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Procedure)
				.WithMany()
				.HasForeignKey(e => e.ProcedureId);

			builder
				.HasOne(e => e.Resource)
				.WithMany()
				.HasForeignKey(e => e.ResourceId);
		}
	}
}
