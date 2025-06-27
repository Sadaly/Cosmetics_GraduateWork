using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserCreate
{
    public sealed class UserUpdateCommandHandler : ICommandHandler<UserUpdateCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserUpdateCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            //Если все поля пустые, то смысла вызывать эту комманду не было (на фронте можно настроить
            //вызов комманды так, чтобы передавался хотя бы один параметр, поэтому если пользователь как-то
            //через интерфейс вызовет это комманду с 3 null-ми, то это можно воспринимать, как ошибку фронта)
            if (request.Email == null && request.Username == null && request.Password == null)
                return Result.Failure<Guid>(ApplicationErrors.UserCommandUpdate.NullValues);

            //Получение пользователя и проверка, существует ли он вообще
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user.IsFailure) return Result.Failure<Guid>(user.Error);

            //Первое условие проверяет, нужно ли обновлять поле
            //Второе условие проверяет нормальный ли результат работы с сущностью Юзера
            if (request.Email != null) {
                user.Value.UpdateEmail(Email.Create(request.Email));
                if (user.IsFailure) return Result.Failure<Guid>(user.Error);
            }

            //Первое условие проверяет, нужно ли обновлять поле
            //Второе условие проверяет нормальный ли результат работы с сущностью Юзера
            if (request.Username != null) {
                user.Value.UpdateUsername(Username.Create(request.Username));
                if (user.IsFailure) return Result.Failure<Guid>(user.Error);
            }

            //Первое условие проверяет, нужно ли обновлять поле
            //Проверку можно не делать, т.к. она уже осуществляется в коммандах Add и Save и если в них поступает
            //неверный результат то они просто передают его дальше
            if (request.Password != null) 
                user.Value.UpdatePassword(PasswordHashed.Create(request.Password));

            var add = await _userRepository.AddAsync(user, cancellationToken);
            var save = await _unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
