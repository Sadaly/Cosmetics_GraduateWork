using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class ResourceTypeConfiguration : IEntityTypeConfiguration<ResourceType>
	{
		public void Configure(EntityTypeBuilder<ResourceType> builder)
		{
			builder.ToTable(TableNames.ResourceType);

			builder.HasKey(u => u.Id);
		}
	}
}
