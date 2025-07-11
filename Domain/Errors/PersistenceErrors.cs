using Domain.Common;
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
        public static readonly Error IncorrectStartIndex = new(
            "IncorrectStartIndex",
            "Некорректный индекс старта взятых элементов. Значение параметра начального индекса не может быть меньше нуля.");
        public static readonly Error IncorrectCount = new(
            "IncorrectCount",
            "Некорректное количество взятых элементов. Значение параметра количества элементов не может быть меньше единицы.");

        public static class Entity<T> where T : BaseEntity
        {
            public static readonly Error IsSoftDeleted = new(
                $"{typeof(T).Name}.IsSoftDeleted",
                "Сущность уже помечена на удаление");
            public static readonly Error ShouldBeSoftDeleted = new(
                $"{typeof(T).Name}.ShouldBeSoftDeleted",
                "Сущность должна быть помечена на удаление");
            public static readonly Error NotFound = new(
                $"{typeof(T).Name}.NotFound",
                $"Сущность типа '{typeof(T).Name}' не найдена");
            public static readonly Error IdEmpty = new(
                $"{typeof(T).Name}.IdEmpty",
                "Не передан Id сущности для поиска в БД");
            public static readonly Error AlreadyExists = new(
                $"{typeof(T).Name}.AlreadyExists",
                $"Сущность типа '{typeof(T).Name}' уже существует");
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
            public static readonly Error UsernameNotUnique = new(
                "User.UsernameNotUnique",
                "Username пользователя не уникален");
            public static readonly Error EmailNotUnique = new(
                "User.EmailNotUnique",
                "Email пользователя не уникален");
            public static readonly Error UpdateChangeNothing = new(
                "User.UpdateChangeNothing",
                "Обновление данных ничего не изменило");
        }
    }
}
