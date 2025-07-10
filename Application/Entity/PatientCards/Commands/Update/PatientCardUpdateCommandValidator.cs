using Domain.ValueObjects;
using FluentValidation;

namespace Application.Entity.PatientCards.Commands.Update
{
    public class PatientCardUpdateCommandValidator : AbstractValidator<PatientCardUpdateCommand>
    {
        public PatientCardUpdateCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotNull()
                .MinimumLength(PhoneNumber.MIN_LENGTH)
                .MinimumLength(PhoneNumber.MIN_LENGTH);
        }
    }
}
