using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
	public static IServiceCollection AddPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options
				.UseNpgsql(configuration.GetConnectionString("Database")));

		services.AddScoped<IAgeChangeRepository, AgeChangeRepository>();

		services.AddScoped<IAgeChangeTypeRepository, AgeChangeTypeRepository>();

		services.AddScoped<IDoctorRepository, DoctorRepository>();

		services.AddScoped<IExternalProcedureRecordRepository, ExternalProcedureRecordRepository>();

		services.AddScoped<IExternalProcedureRecordTypeRepository, ExternalProcedureRecordTypeRepository>();

		services.AddScoped<IHealthCondRepository, HealthCondRepository>();

		services.AddScoped<IHealthCondTypeRepository, HealthCondTypeRepository>();

		services.AddScoped<INotificationRepository, NotificationRepository>();

		services.AddScoped<IPatientCardRepository, PatientCardRepository>();

		services.AddScoped<IPatientRepository, PatientRepository>();

		services.AddScoped<IPatientSpecificsRepository, PatientSpecificsRepository>();

		services.AddScoped<IProcedureRepository, ProcedureRepository>();

		services.AddScoped<IProcedureTypeRepository, ProcedureTypeRepository>();

		services.AddScoped<IReservedDateRepository, ReservedDateRepository>();

		services.AddScoped<ISkinCareRepository, SkinCareRepository>();

		services.AddScoped<ISkinCareTypeRepository, SkinCareTypeRepository>();

		services.AddScoped<ISkinFeatureRepository, SkinFeatureRepository>();

		services.AddScoped<ISkinFeatureTypeRepository, SkinFeatureTypeRepository>();

		services.AddScoped<IUserRepository, UserRepository>();

		return services;
	}
}
