using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Patients.Queries.Get
{
	internal class PatientGetQueryHandler(IPatientRepository patientRepository) : IQueryHandler<PatientGetQuery, PatientResponse>
	{
		public async Task<Result<PatientResponse>> Handle(PatientGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await patientRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<PatientResponse>(entity.Error);

			var response = new PatientResponse(entity.Value);

			return response;
		}
	}
}
