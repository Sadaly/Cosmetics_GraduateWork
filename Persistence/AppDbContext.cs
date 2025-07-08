using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientCard> PatientCards { get; set; }
        public DbSet<SkinFeature> SkinFeatures { get; set; }
        public DbSet<SkinCare> SkinCares { get; set; }
        public DbSet<SkinCareType> SkinCareTypes { get; set; }
        public DbSet<HealthCond> HealthConds { get; set; }
        public DbSet<HealthCondType> HealthCondTypes { get; set; }
        public DbSet<AgeChange> AgeChanges { get; set; }
        public DbSet<AgeChangeType> AgeChangeTypes { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<ExternalProcedureRecord> ExternalProcedureRecords { get; set; }
        public DbSet<ExternalProcedureRecordType> ExternalProcedureRecordTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PatientSpecifics> PatientSpecificses { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureType> ProcedureTypes { get; set; }
        public DbSet<ReservedDate> ReservedDates { get; set; }
        public DbSet<SkinFeatureType> SkinFeatureTypes { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplyConfiguration(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=postgres;");
        }
        
        private static void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.AgeChangeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AgeChangeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ExternalProcedureRecordConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ExternalProcedureRecordTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.HealthCondConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.HealthCondTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PatientCardConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PatientConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PatientSpecificsConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ProcedureTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ReservedDateConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinCareConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinCareTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinFeatureConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinFeatureTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
        }
    }
}
