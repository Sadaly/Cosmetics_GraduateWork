using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class ProcedureTypesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(ProcedureTypesController.Create) },
            { nameof(ProcedureTypesController.Get) },
            { nameof(ProcedureTypesController.GetAll) },
            { nameof(ProcedureTypesController.Take) },
            { nameof(ProcedureTypesController.Update) },
            { nameof(ProcedureTypesController.RemoveById) },
        };
    }
}
