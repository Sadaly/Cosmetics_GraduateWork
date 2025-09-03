using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class ReservedDatesControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(ReservedDatesController.Create) },
			{ nameof(ReservedDatesController.Get) },
			{ nameof(ReservedDatesController.GetAll) },
			{ nameof(ReservedDatesController.Take) },
			{ nameof(ReservedDatesController.RemoveById) },
		};
	}
}
