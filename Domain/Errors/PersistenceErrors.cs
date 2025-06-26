using Domain.Shared;
namespace Domain.Errors
{
    /// <summary>
    /// Класс для обозначения ошибок уровня работы с базой данных. Сюда могут входить
    /// как логические ошибки (например обращение к некорректным данным), так и другие
    /// (например уведомление о том что почта пользователя может быть занята)
    /// </summary>
    public static class PersistenceErrors
    {
        public static class Entity
        {
            public static readonly Error IsSoftDeleted = new(
                "Entity.IsSoftDeleted",
                "Сущность помечена на удаление");
        }
            public static class User
        {
            public static readonly Error EmailAlreadyInUse = new(
                "User.EmailAlreadyInUse",
                "Почта уже используется");
            public static readonly Error UsernameAlreadyInUse = new(
                "User.UsernameAlreadyInUse",
                "Имя пользователя уже используется");
            public static readonly Error IncorrectUsernameOrPassword = new(
                "User.IncorrectUsernameOrPassword",
                "Неправильное имя пользователя или пароль");
            public static readonly Error NotFound = new(
                "User.NotFound",
                "Пользователь не найден");
            public static readonly Error UsernameNotUnique = new(
                "User.UsernameNotUnique",
                "Username пользователя не уникален");
            public static readonly Error EmailNotUnique = new(
                "User.EmailNotUnique",
                "Email пользователя не уникален");
            public static readonly Error AlreadyExists = new(
                "User.AlreadyExists",
                "Пользователь уже существует");
        }
    }
}
