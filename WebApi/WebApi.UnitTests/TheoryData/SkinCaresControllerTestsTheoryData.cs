using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class SkinCaresControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(SkinCaresController.Create) },
            { nameof(SkinCaresController.Get) },
            { nameof(SkinCaresController.GetAll) },
            { nameof(SkinCaresController.Take) },
            { nameof(SkinCaresController.ChangeType) },
            { nameof(SkinCaresController.RemoveById) },
        };
    }
}
