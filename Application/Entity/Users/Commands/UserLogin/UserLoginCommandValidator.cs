using FluentValidation;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserLogin
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().Must(email 
                => email.Split('@').Length == 2);

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(PasswordHashed.MIN_LENGTH)
                .MaximumLength(PasswordHashed.MAX_LENGTH);
        }
	}
}
