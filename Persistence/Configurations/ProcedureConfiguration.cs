using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
	{
		public void Configure(EntityTypeBuilder<Procedure> builder)
		{
			builder.ToTable(TableNames.Procedure);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Type)
				.WithMany()
				.HasForeignKey(e => e.TypeId);


			builder
				.HasOne(p => p.Notification)
				.WithOne(n => n.Procedure)
				.HasForeignKey<Notification>(n => n.ProcedureId)
				.IsRequired(false);
		}
	}
}
