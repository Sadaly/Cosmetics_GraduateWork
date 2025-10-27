using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientCards.Queries.Get
{
	internal class PatientCardGetQueryHandler(IPatientCardRepository patientcardRepository) : IQueryHandler<PatientCardGetQuery, PatientCardResponse>
	{
		public async Task<Result<PatientCardResponse>> Handle(PatientCardGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await patientcardRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken, Domain.Abstractions.FetchMode.Include);
			if (entity.IsFailure) return Result.Failure<PatientCardResponse>(entity.Error);

			var response = new PatientCardResponse(entity.Value);

			return response;
		}
	}
}
