using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
	{
		public void Configure(EntityTypeBuilder<Patient> builder)
		{
			builder.ToTable(TableNames.Patient);

			builder.HasKey(p => p.Id);

			builder.HasOne(p => p.Card)
				.WithOne(pc => pc.Patient)
				.HasForeignKey<PatientCard>(pc => pc.PatientId);

		}
	}
}
