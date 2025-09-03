using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
	public class ProceduresControllerTestsTheoryData
	{
		public static TheoryData<string> UserOnlyPolicyMethodsName = new()
		{
			{ nameof(ProceduresController.Create) },
			{ nameof(ProceduresController.Get) },
			{ nameof(ProceduresController.GetAll) },
			{ nameof(ProceduresController.Take) },
			{ nameof(ProceduresController.ChangeType) },
			{ nameof(ProceduresController.RemoveById) },
			{ nameof(ProceduresController.AssignDoctor) },
			{ nameof(ProceduresController.RemoveDoctor) },
			{ nameof(ProceduresController.UpdateDate) },
		};
	}
}
