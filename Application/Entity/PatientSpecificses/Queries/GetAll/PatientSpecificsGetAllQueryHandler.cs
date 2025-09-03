using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientSpecificses.Queries.GetAll
{
	internal class PatientSpecificsGetAllQueryHandler(IPatientSpecificsRepository patientSpecificsRepository) : IQueryHandler<PatientSpecificsGetAllQuery, List<PatientSpecificsResponse>>
	{
		public async Task<Result<List<PatientSpecificsResponse>>> Handle(PatientSpecificsGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await patientSpecificsRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await patientSpecificsRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<PatientSpecificsResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new PatientSpecificsResponse(u)).ToList();

			return listRes;
		}
	}
}
