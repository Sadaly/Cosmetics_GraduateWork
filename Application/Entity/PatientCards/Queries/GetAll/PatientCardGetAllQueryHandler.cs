using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientCards.Queries.GetAll
{
	internal class PatientCardGetAllQueryHandler(IPatientCardRepository patientcardRepository) : IQueryHandler<PatientCardGetAllQuery, List<PatientCardResponse>>
	{
		public async Task<Result<List<PatientCardResponse>>> Handle(PatientCardGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await patientcardRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await patientcardRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<PatientCardResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new PatientCardResponse(u)).ToList();

			return listRes;
		}
	}
}
