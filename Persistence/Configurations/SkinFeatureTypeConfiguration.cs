using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class SkinFeatureTypeConfiguration : IEntityTypeConfiguration<SkinFeatureType>
	{
		public void Configure(EntityTypeBuilder<SkinFeatureType> builder)
		{
			builder.ToTable(TableNames.SkinFeatureType);

			builder.HasKey(sft => sft.Id);
		}
	}
}
