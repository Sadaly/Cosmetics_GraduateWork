using Domain.Shared;
namespace Domain.Errors
{
    public static class ApplicationErrors
    {
        public static class UserCommandUpdate
        {
            public static readonly Error NullValues = new(
                "UserCommandUpdate.NullValues",
                "Переданные поля не содержат данных, обновить пользователя невозможно");

            public static readonly Error EmailAlreadyInUse = new(
                "User.Update.EmailAlreadyInUse",
                "Почта уже используется этим пользователем");

            public static readonly Error UsernameAlreadyInUse = new(
                "User.Update.UsernameAlreadyInUse",
                "Это имя уже используется этим пользователем");
        }
    }
}
