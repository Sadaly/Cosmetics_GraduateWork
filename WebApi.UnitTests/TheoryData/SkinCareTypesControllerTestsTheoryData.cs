using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class SkinCareTypesControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(SkinCareTypesController.Create) },
			{ nameof(SkinCareTypesController.Get) },
			{ nameof(SkinCareTypesController.GetAll) },
			{ nameof(SkinCareTypesController.Take) },
			{ nameof(SkinCareTypesController.Update) },
			{ nameof(SkinCareTypesController.RemoveById) },
		};
	}
}
