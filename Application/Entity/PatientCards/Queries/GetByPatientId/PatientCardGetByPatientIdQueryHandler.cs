using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientCards.Queries.GetByPatientId
{
	internal class PatientCardGetByPatientIdQueryHandler(IPatientCardRepository patientcardRepository) : IQueryHandler<PatientCardGetByPatientIdQuery, PatientCardResponse>
	{
		public async Task<Result<PatientCardResponse>> Handle(PatientCardGetByPatientIdQuery request, CancellationToken cancellationToken)
		{
			var entity = await patientcardRepository.GetByPredicateAsync(pc => pc.PatientId == request.PatientId, cancellationToken);
			if (entity.IsFailure) return Result.Failure<PatientCardResponse>(entity.Error);

			var response = new PatientCardResponse(entity.Value);

			return response;
		}
	}
}
