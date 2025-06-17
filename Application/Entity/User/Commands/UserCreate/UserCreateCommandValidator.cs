using FluentValidation;
using Domain.Errors;
using Domain.ValueObjects;
using Domain.Shared;

namespace Application.Entity.Users.Commands.UserCreate
{
    public class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
    {
        public UserCreateCommandValidator() 
        {
            RuleFor(x => x.Username).NotEmpty()
                .MinimumLength(Username.MIN_LENGTH)
                .MaximumLength(Username.MAX_LENGTH); ;

            RuleFor(x => x.Email).NotEmpty().Must((email) =>
            {
                // Проверка ввода на корректность формата
                var split = email.Split('@');
                if (split.Length != 2) return false;
                if (split[0].Length < Email.FIRST_PART_MIN_LENGTH
                    || split[1].Length < Email.SECOND_PART_MIN_LENGTH)
                    return false;
                return true;
            }).WithMessage(DomainErrors.Email.InvalidFormat)  //Сообщение о неправильном формате
                .MinimumLength(Email.MIN_LENGTH)
                .MaximumLength(Email.MAX_LENGTH); 

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(PasswordHashed.MIN_LENGTH)
                .MaximumLength(PasswordHashed.MAX_LENGTH);
        }
	}
}
