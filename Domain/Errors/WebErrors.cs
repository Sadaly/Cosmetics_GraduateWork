using Domain.Shared;
namespace Domain.Errors
{
    public static class WebErrors
    {
        public static class UserController
        {
            public static class UpdateSelf
            {
                public static readonly Error EmptyId = new(
                "UserController.UpdateSelf.EmptyId",
                "Передан пустой Id, невозможно обновить данные пользователя");
            }
        }
    }
}
