using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Testcontainers.PostgreSql;

namespace WebApi.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase("postgres")
                .WithUsername("postgres")
                .WithPassword("123456")
                .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Вызывается единожды, 2-ым в стеке вызовов
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(AppDbContext));

                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options
                        .UseNpgsql(_dbContainer.GetConnectionString())
                );
                services.AddScoped<TransactionalTestDatabase>();
            });
        }

        //Вызывается единожды, самым 1-ым в стеке вызовов
        public Task InitializeAsync() { return _dbContainer.StartAsync(); }

        //Вызывается единожды, самым последним в стеке вызовов
        public new Task DisposeAsync() { return _dbContainer.StopAsync(); }
    }
}
