using FluentValidation;
using Domain.Errors;
using Domain.ValueObjects;
using Domain.Shared;

namespace Application.Entity.Users.Commands.UserCreate
{
    public class UserUpdateCommandValidator : AbstractValidator<UserUpdateCommand>
    {
        public UserUpdateCommandValidator()
        {
            //Если значения null, мы их никак не проверяем (предполагается, что пользователь оставил часть пустыми,
            //а часть заполнил. Это проверяется на фронте и в обработчике комманды)

            RuleFor(x => x.Username)

                //Проверка максимальной длины
                .Must(username => {
                    if (!string.IsNullOrEmpty(username)) if (username.Length > Username.MAX_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.Username.TooLong)

                //Проверка минимальной длины
                .Must(username => {
                    if (!string.IsNullOrEmpty(username)) if (username.Length < Username.MIN_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.Username.TooShort)
                ;

            RuleFor(x => x.Email)

                //Проверка формата
                .Must(email => {
                    if (!string.IsNullOrEmpty(email)) return Email.IsValidFormat(email);
                    return true;
                }).WithMessage(DomainErrors.Email.InvalidFormat)

                //Проверка максимальной длины
                .Must(email => {
                    if (!string.IsNullOrEmpty(email)) if (email.Length > Email.MAX_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.Email.TooLong)

                //Проверка минимальной длины
                .Must(email => {
                    if (!string.IsNullOrEmpty(email)) if (email.Length < Email.MIN_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.Email.TooShort)
                ;

            RuleFor(x => x.Password).NotEmpty()
                //Проверка максимальной длины
                .Must(password => {
                    if (!string.IsNullOrEmpty(password)) if (password.Length > PasswordHashed.MAX_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.PasswordHashed.TooLong)

                //Проверка минимальной длины
                .Must(password => {
                    if (!string.IsNullOrEmpty(password)) if (password.Length < PasswordHashed.MIN_LENGTH) return false;
                    return true;
                }).WithMessage(DomainErrors.PasswordHashed.TooShort)
                ;
        }
    }
}
