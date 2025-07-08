using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class DoctorRepository(AppDbContext dbContext) 
        : TRepository<Doctor>(dbContext), IDoctorRepository
    {
        private protected override IQueryable<Doctor> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.Procedures);
    }
}
