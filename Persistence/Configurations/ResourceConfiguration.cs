using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ResourceConfiguration : IEntityTypeConfiguration<Resource>
	{
		public void Configure(EntityTypeBuilder<Resource> builder)
		{
			builder.ToTable(TableNames.Resource);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Type)
				.WithMany()
				.HasForeignKey(e => e.TypeId);

			//builder.HasMany(r => r.Procedures)
			//	.WithMany(p => p.UsedResources)
			//	.UsingEntity<ProcedureResource>();
		}
	}
}
