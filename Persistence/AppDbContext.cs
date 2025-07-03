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
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PatientConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PatientCardConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinFeatureConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SkinFeatureTypeConfiguration());
        }
    }
}
