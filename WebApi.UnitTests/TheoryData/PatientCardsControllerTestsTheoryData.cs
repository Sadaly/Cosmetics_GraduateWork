using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class PatientCardsControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(PatientCardsController.Get) },
			{ nameof(PatientCardsController.GetAll) },
			{ nameof(PatientCardsController.Take) },
			{ nameof(PatientCardsController.Update) },
		};
	}
}
