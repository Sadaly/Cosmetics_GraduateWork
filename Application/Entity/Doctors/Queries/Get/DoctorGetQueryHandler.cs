using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Doctors.Queries.Get
{
    internal class DoctorGetQueryHandler(IDoctorRepository doctorRepository) : IQueryHandler<DoctorGetQuery, DoctorResponse>
    {
        public async Task<Result<DoctorResponse>> Handle(DoctorGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await doctorRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<DoctorResponse>(entity.Error);

            var response = new DoctorResponse(entity.Value);

            return response;
        }
    }
}
