using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class ExternalProcedureRecordsControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(ExternalProcedureRecordsController.Create) },
            { nameof(ExternalProcedureRecordsController.Get) },
            { nameof(ExternalProcedureRecordsController.GetAll) },
            { nameof(ExternalProcedureRecordsController.Take) },
            { nameof(ExternalProcedureRecordsController.ChangeType) },
            { nameof(ExternalProcedureRecordsController.RemoveById) },
        };
    }
}
