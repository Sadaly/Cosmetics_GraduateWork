using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class NotificationsControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(NotificationsController.Create) },
			{ nameof(NotificationsController.Get) },
			{ nameof(NotificationsController.GetAll) },
			{ nameof(NotificationsController.Take) },
			{ nameof(NotificationsController.UpdateMessage) },
			{ nameof(NotificationsController.UpdatePhonenumber) },
			{ nameof(NotificationsController.RemoveById) },
		};
	}
}
