using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Persistence;
using Testcontainers.PostgreSql;

namespace Application.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _dbContainer;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options
                        .UseNpgsql("User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=postgres;")
                );
                services.AddScoped<TransactionalTestDatabase>();
            });
        }

        public async Task InitializeAsync() {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase(Guid.NewGuid().ToString("N"))
                .WithName(Guid.NewGuid().ToString("N"))
                .WithUsername("postgres")
                .WithPassword("123456")
                .Build();

            await _dbContainer.StartAsync();
        } 
        public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();
    }
}
