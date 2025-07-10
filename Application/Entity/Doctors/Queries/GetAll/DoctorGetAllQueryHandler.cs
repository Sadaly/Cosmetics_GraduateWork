using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Doctors.Queries.GetAll
{
    internal class DoctorGetAllQueryHandler(IDoctorRepository doctorRepository) : IQueryHandler<DoctorGetAllQuery, List<DoctorResponse>>
    {
        public async Task<Result<List<DoctorResponse>>> Handle(DoctorGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await doctorRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await doctorRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<DoctorResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new DoctorResponse(u)).ToList();

            return listRes;
        }
    }
}
