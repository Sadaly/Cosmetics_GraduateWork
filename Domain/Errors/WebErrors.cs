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
			public static class RemoveById
			{
				public static readonly Error SelfDeleteFobbiden = new(
				"UserController.RemoveById.SelfDeleteFobbiden",
				"Запрещено удалять самого себя из системы. Создайте нового пользователя и удалите себя через него");
				public static readonly Error EmptyId = new(
				"UserController.RemoveById.EmptyId",
				"Передан пустой Id, невозможно обновить данные пользователя");
			}
		}
	}
}
