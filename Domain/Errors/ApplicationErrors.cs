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
        }
    }
}
