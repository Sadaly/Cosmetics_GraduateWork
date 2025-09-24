using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ResourceOperationConfiguration : IEntityTypeConfiguration<ResourceOperation>
	{
		public void Configure(EntityTypeBuilder<ResourceOperation> builder)
		{
			builder.ToTable(TableNames.ResourceOperation);

			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Resource)
				.WithMany()
				.HasForeignKey(e => e.ResourceId);
		}
	}
}
