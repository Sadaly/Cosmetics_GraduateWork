using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientSpecificses.Commands.Update
{
	internal class PatientSpecificsUpdateCommandHandler(IPatientSpecificsRepository patientspecificsRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientSpecificsUpdateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(PatientSpecificsUpdateCommand request, CancellationToken cancellationToken)
		{
			var patientspecifics = await patientspecificsRepository.GetByIdAsync(request.Id, cancellationToken);
			if (patientspecifics.IsFailure) return Result.Failure<Guid>(patientspecifics.Error);

			patientspecifics.Value.Update(request.Sleep, request.Diet, request.Sport, request.WorkEnviroment);

			var add = await patientspecificsRepository.UpdateAsync(patientspecifics, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
