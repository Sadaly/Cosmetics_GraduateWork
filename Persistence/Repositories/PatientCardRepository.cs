using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientCardRepository : TRepository<PatientCard>, IPatientCardRepository
    {
        public PatientCardRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
