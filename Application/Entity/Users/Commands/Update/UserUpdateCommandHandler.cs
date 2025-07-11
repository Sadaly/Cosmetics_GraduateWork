using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.Update
{
    public sealed class UserUpdateCommandHandler(IJwtProvider jwtProvider, IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<UserUpdateCommand, string>
    {
        public async Task<Result<string>> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            //Если все поля пустые, то смысла вызывать эту комманду не было (на фронте можно настроить
            //вызов комманды так, чтобы передавался хотя бы один параметр, поэтому если пользователь как-то
            //через интерфейс вызовет это комманду с 3 null-ми, то это можно воспринимать, как ошибку фронта)
            if (request.Email == null && request.Username == null && request.Password == null)
                return Result.Failure<string>(ApplicationErrors.UserUpdateCommand.NullValues);

            //Получение пользователя и проверка, существует ли он вообще
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken, FetchMode.NoTracking);
            if (user.IsFailure) return Result.Failure<string>(user.Error);

            //Первое условие проверяет, нужно ли обновлять поле
            //Второе условие проверяет нормальный ли результат работы с сущностью Юзера
            if (request.Email != null) {
                var email = Email.Create(request.Email);
                user.Value.UpdateEmail(email);
                if (user.IsFailure) return Result.Failure<string>(user.Error);
            }

            //Первое условие проверяет, нужно ли обновлять поле
            //Второе условие проверяет нормальный ли результат работы с сущностью Юзера
            if (request.Username != null) {
                var username = Username.Create(request.Username);
                user.Value.UpdateUsername(username);
                if (user.IsFailure) return Result.Failure<string>(user.Error);
            }

            //Первое условие проверяет, нужно ли обновлять поле
            //Проверку корректности Result можно не делать, т.к. она уже осуществляется в коммандах Add и Save и если в них поступает
            //неверный результат то они просто передают его дальше
            if (request.Password != null) 
                user.Value.UpdatePassword(PasswordHashed.Create(request.Password));

            var update = await userRepository.UpdateAsync(user, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(update, cancellationToken);
            string token = jwtProvider.Generate(user.Value);
            return save.IsSuccess
                ? token
                : Result.Failure<string>(save.Error);
        }
    }
}
