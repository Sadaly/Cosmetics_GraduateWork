using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class SkinFeatureTypesControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(SkinFeatureTypesController.Create) },
            { nameof(SkinFeatureTypesController.Get) },
            { nameof(SkinFeatureTypesController.GetAll) },
            { nameof(SkinFeatureTypesController.Take) },
            { nameof(SkinFeatureTypesController.Update) },
            { nameof(SkinFeatureTypesController.RemoveById) },
        };
    }
}
