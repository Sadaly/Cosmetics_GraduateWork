using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace Application.IntegrationTests
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly AppDbContext dbContext;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _scope.ServiceProvider.GetRequiredService<TransactionalTestDatabase>();
        }
    }
}
