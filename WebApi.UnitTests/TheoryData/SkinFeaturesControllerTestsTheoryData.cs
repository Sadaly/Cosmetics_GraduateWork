using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class SkinFeaturesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(SkinFeaturesController.Create) },
            { nameof(SkinFeaturesController.Get) },
            { nameof(SkinFeaturesController.GetAll) },
            { nameof(SkinFeaturesController.Take) },
            { nameof(SkinFeaturesController.ChangeType) },
            { nameof(SkinFeaturesController.RemoveById) },
        };
    }
}
