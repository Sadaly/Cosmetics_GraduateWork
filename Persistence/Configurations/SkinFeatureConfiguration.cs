using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class SkinFeatureConfiguration : IEntityTypeConfiguration<SkinFeature>
	{
		public void Configure(EntityTypeBuilder<SkinFeature> builder)
		{
			builder.ToTable(TableNames.SkinFeature);

			builder.HasKey(sf => sf.Id);
			builder
				.HasOne(sf => sf.Type)
				.WithMany()
				.HasForeignKey(sf => sf.TypeId);
		}
	}
}
