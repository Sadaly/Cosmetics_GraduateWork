using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class AgeChangeTypesControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(AgeChangeTypesController.Create) },
			{ nameof(AgeChangeTypesController.Get) },
			{ nameof(AgeChangeTypesController.GetAll) },
			{ nameof(AgeChangeTypesController.Take) },
			{ nameof(AgeChangeTypesController.Update) },
			{ nameof(AgeChangeTypesController.RemoveById) },
		};
	}
}
