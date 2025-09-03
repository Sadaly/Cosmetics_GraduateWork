using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	internal class PatientCardConfiguration : IEntityTypeConfiguration<PatientCard>
	{
		public void Configure(EntityTypeBuilder<PatientCard> builder)
		{
			builder.ToTable(TableNames.PatientCard);

			builder.HasKey(pc => pc.Id);

			builder.HasOne(p => p.Specifics)
				.WithOne(pc => pc.PatientCard)
				.HasForeignKey<PatientSpecifics>(pc => pc.PatientCardId);

			builder
				.HasMany(pc => pc.AgeChanges)
				.WithOne(ac => ac.PatientCard)
				.HasForeignKey(ac => ac.PatientCardId);
			builder
				.HasMany(pc => pc.ExternalProcedureRecords)
				.WithOne(epr => epr.PatientCard)
				.HasForeignKey(epr => epr.PatientCardId);
			builder
				.HasMany(pc => pc.HealthConds)
				.WithOne(hc => hc.PatientCard)
				.HasForeignKey(hc => hc.PatientCardId);
			builder
				.HasMany(pc => pc.Procedures)
				.WithOne(p => p.PatientCard)
				.HasForeignKey(p => p.PatientCardId);
			builder
				.HasMany(pc => pc.SkinCares)
				.WithOne(sc => sc.PatientCard)
				.HasForeignKey(sc => sc.PatientCardId);
			builder
				.HasMany(pc => pc.SkinFeatures)
				.WithOne(sf => sf.PatientCard)
				.HasForeignKey(sf => sf.PatientCardId);
		}
	}
}
