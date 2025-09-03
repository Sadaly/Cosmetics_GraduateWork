using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
	{
		public void Configure(EntityTypeBuilder<Doctor> builder)
		{
			builder.ToTable(TableNames.Doctor);

			builder.HasKey(u => u.Id);

			builder
				.HasMany(d => d.Procedures)
				.WithOne(p => p.Doctor)
				.HasForeignKey(p => p.DoctorId)
				.IsRequired(false);
		}
	}
}
