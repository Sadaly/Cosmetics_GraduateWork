using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Patients.Queries.GetAll
{
    internal sealed class PatientsGetAllQueryHandler(IPatientRepository patientRepository) : IQueryHandler<PatientsGetAllQuery, List<PatientResponse>>
    {
        private readonly IPatientRepository _patientRepository = patientRepository;

        public async Task<Result<List<PatientResponse>>> Handle(PatientsGetAllQuery request, CancellationToken cancellationToken)
        {
            var patients = request.StartIndex == null || request.Count == null
                ? await _patientRepository.GetAllAsync(request.Query.Predicate, cancellationToken, Domain.Abstractions.FetchMode.Include)
                : await _patientRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken, Domain.Abstractions.FetchMode.Include);
            if (patients.IsFailure) return Result.Failure<List<PatientResponse>>(patients.Error);

            var listRes = patients.Value.Select(p => new PatientResponse(p)).ToList();

            return listRes;
        }
    }
}