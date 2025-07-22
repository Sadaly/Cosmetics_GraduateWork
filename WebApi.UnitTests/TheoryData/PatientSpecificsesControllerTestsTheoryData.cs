using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class PatientSpecificsesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(PatientSpecificsesController.Get) },
            { nameof(PatientSpecificsesController.GetAll) },
            { nameof(PatientSpecificsesController.Take) },
            { nameof(PatientSpecificsesController.Update) },
            { nameof(PatientSpecificsesController.Create) },
        };
    }
}
