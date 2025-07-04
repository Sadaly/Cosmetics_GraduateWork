using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Patients.Queries.GetAll
{
    internal sealed class PatientsGetAllQueryHandler : IQueryHandler<PatientsGetAllQuery, List<PatientResponses>>
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsGetAllQueryHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<Result<List<PatientResponses>>> Handle(PatientsGetAllQuery request, CancellationToken cancellationToken)
        {
            var patients = request.Predicate == null
                ? await _patientRepository.GetAllAsync(cancellationToken, Domain.Abstractions.FetchMode.Include)
                : await _patientRepository.GetAllAsync(request.Predicate, cancellationToken, Domain.Abstractions.FetchMode.Include);

            if (patients.IsFailure) return Result.Failure<List<PatientResponses>>(patients.Error);

            var listRes = patients.Value.Select(u => new PatientResponses(u)).ToList();

            return listRes;
        }
    }
}