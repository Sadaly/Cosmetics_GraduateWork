using WebApi.Controllers;

namespace WebApi.UnitTests.TheoryData
{
    public class UsersControllerTestsTheoryData
    {
        public static TheoryData<string> UserOnlyPolicyMethodsName = new()
        {
            { nameof(UsersController.Create) },
            { nameof(UsersController.GetById) },
            { nameof(UsersController.GetAll) },
            { nameof(UsersController.Take) },
            { nameof(UsersController.UpdateSelf) },
            { nameof(UsersController.RemoveById) },
        };
    }
}
