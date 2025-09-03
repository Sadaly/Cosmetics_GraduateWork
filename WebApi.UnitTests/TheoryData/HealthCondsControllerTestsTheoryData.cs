using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class HealthCondsControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(HealthCondsController.Create) },
			{ nameof(HealthCondsController.Get) },
			{ nameof(HealthCondsController.GetAll) },
			{ nameof(HealthCondsController.Take) },
			{ nameof(HealthCondsController.ChangeType) },
			{ nameof(HealthCondsController.RemoveById) },
		};
	}
}
