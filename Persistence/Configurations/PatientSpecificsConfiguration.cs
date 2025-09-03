using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class PatientSpecificsConfiguration : IEntityTypeConfiguration<PatientSpecifics>
	{
		public void Configure(EntityTypeBuilder<PatientSpecifics> builder)
		{
			builder.ToTable(TableNames.PatientSpecifics);

			builder.HasKey(u => u.Id);
		}
	}
}
