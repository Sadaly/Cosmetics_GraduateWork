using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class PatientsControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(PatientsController.Create) },
            { nameof(PatientsController.Get) },
            { nameof(PatientsController.GetAll) },
            { nameof(PatientsController.Take) },
            { nameof(PatientsController.Update) },
            { nameof(PatientsController.RemoveById) },
        };
    }
}
