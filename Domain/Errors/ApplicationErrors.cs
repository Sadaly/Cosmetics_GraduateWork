using Domain.Shared;
namespace Domain.Errors
{
    public static class ApplicationErrors
    {
        public static class UserUpdateCommand
        {
            public static readonly Error NullValues = new(
                "UserUpdateCommand.NullValues",
                "Переданные поля не содержат данных, обновить пользователя невозможно");

            public static readonly Error EmailAlreadyInUse = new(
                "UserUpdateCommand.EmailAlreadyInUse",
                "Почта уже используется этим пользователем");

            public static readonly Error UsernameAlreadyInUse = new(
                "UserUpdateCommand.UsernameAlreadyInUse",
                "Это имя уже используется этим пользователем");
        }
        public static class ProcedureCreateCommand
        {
            public static readonly Error DateReserved = new(
                "ProcedureCreateCommand.DateReserved",
                "В указанную дату нельзя записать процедуру");
        }
        public static class PatientSpecificsCreateCommand
        {
            public static readonly Error AlreadyExists = new(
                "PatientSpecificsCreateCommand.AlreadyExists",
                "Особенности пациента уже записаны. Повторная запись запрещена. Обновите предыдущую");
        }
    }
}
