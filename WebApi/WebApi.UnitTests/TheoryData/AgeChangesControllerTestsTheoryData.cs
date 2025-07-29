using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class AgeChangesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(AgeChangesController.Create) },
            { nameof(AgeChangesController.Get) },
            { nameof(AgeChangesController.GetAll) },
            { nameof(AgeChangesController.Take) },
            { nameof(AgeChangesController.ChangeType) },
            { nameof(AgeChangesController.RemoveById) },
        };
    }
}
