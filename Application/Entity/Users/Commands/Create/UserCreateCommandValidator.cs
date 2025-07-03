using FluentValidation;
using Domain.Errors;
using Domain.ValueObjects;
using Domain.Shared;
using Application.Entity.Users.Commands.Create;

namespace Application.Entity.Users.Commands.UserCreate
{
    public class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
    {
        public UserCreateCommandValidator() 
        {
            RuleFor(x => x.Username).NotEmpty()
                .MinimumLength(Username.MIN_LENGTH)
                .MaximumLength(Username.MAX_LENGTH);

            RuleFor(x => x.Email).NotEmpty().Must(email => { return Email.IsValidFormat(email); })
                .WithMessage(DomainErrors.Email.InvalidFormat)
                .MinimumLength(Email.MIN_LENGTH)
                .MaximumLength(Email.MAX_LENGTH); 

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(PasswordHashed.MIN_LENGTH)
                .MaximumLength(PasswordHashed.MAX_LENGTH);
        }
	}
}
