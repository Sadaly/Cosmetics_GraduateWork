using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class SkinCareTypeConfiguration : IEntityTypeConfiguration<SkinCareType>
	{
		public void Configure(EntityTypeBuilder<SkinCareType> builder)
		{
			builder.ToTable(TableNames.SkinCareType);

			builder.HasKey(u => u.Id);
		}
	}
}
