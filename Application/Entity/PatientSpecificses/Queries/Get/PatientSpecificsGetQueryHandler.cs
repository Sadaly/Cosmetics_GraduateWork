using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientSpecificses.Queries.Get
{
	internal class PatientSpecificsGetQueryHandler(IPatientSpecificsRepository patientSpecificsRepository) : IQueryHandler<PatientSpecificsGetQuery, PatientSpecificsResponse>
	{
		public async Task<Result<PatientSpecificsResponse>> Handle(PatientSpecificsGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await patientSpecificsRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<PatientSpecificsResponse>(entity.Error);

			var response = new PatientSpecificsResponse(entity.Value);

			return response;
		}
	}
}
