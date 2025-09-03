using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.AssignDoctor
{
	internal class ProcedureAssignDoctorCommandHandler(IDoctorRepository doctorRepository, IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureAssignDoctorCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ProcedureAssignDoctorCommand request, CancellationToken cancellationToken)
		{
			var doc = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
			if (doc.IsFailure) return Result.Failure<Guid>(doc.Error);

			var ent = await procedureRepository.GetByIdAsync(request.ProcedureId, cancellationToken);
			if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

			var update = ent.Value.AssignDoctor(doc);

			var add = await procedureRepository.UpdateAsync(update, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
