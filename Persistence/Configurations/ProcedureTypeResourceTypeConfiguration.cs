using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ProcedureTypeResourceTypeConfiguration : IEntityTypeConfiguration<ProcedureTypeResourceType>
	{
		public void Configure(EntityTypeBuilder<ProcedureTypeResourceType> builder)
		{
			builder.ToTable(TableNames.ProcedureTypeResourceType);

			builder.HasKey(u => u.Id);
			builder.HasOne(ptrt => ptrt.ProcedureType)
				.WithMany(pt => pt.ResourceTypes)
				.HasForeignKey(ptrt => ptrt.ProcedureTypeId);

			builder.HasOne(ptrt => ptrt.ResourceType)
				.WithMany(rt => rt.ProcedureTypes)
				.HasForeignKey(ptrt => ptrt.ResourceTypeId);

		}
	}
}
