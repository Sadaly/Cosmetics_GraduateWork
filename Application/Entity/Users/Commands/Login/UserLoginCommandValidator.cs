using FluentValidation;
using Domain.ValueObjects;
using Domain.Errors;
using Application.Entity.Users.Commands.Login;

namespace Application.Entity.Users.Commands.UserLogin
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().Must(email => { return Email.IsValidFormat(email); })
                .WithMessage(DomainErrors.Email.InvalidFormat);

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(PasswordHashed.MIN_LENGTH)
                .MaximumLength(PasswordHashed.MAX_LENGTH);
        }
	}
}
