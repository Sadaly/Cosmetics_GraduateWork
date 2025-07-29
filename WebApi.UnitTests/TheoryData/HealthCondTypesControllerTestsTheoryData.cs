using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class HealthCondTypesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(HealthCondTypesController.Create) },
            { nameof(HealthCondTypesController.Get) },
            { nameof(HealthCondTypesController.GetAll) },
            { nameof(HealthCondTypesController.Take) },
            { nameof(HealthCondTypesController.Update) },
            { nameof(HealthCondTypesController.RemoveById) },
        };
    }
}
