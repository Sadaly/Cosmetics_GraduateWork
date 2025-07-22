using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class ExternalProcedureRecordTypesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(ExternalProcedureRecordTypesController.Create) },
            { nameof(ExternalProcedureRecordTypesController.Get) },
            { nameof(ExternalProcedureRecordTypesController.GetAll) },
            { nameof(ExternalProcedureRecordTypesController.Take) },
            { nameof(ExternalProcedureRecordTypesController.Update) },
            { nameof(ExternalProcedureRecordTypesController.RemoveById) },
        };
    }
}
