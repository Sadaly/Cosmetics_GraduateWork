using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class DoctorsControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(DoctorsController.Create) },
			{ nameof(DoctorsController.Get) },
			{ nameof(DoctorsController.GetAll) },
			{ nameof(DoctorsController.Take) },
			{ nameof(DoctorsController.Update) },
			{ nameof(DoctorsController.RemoveById) },
		};
	}
}
