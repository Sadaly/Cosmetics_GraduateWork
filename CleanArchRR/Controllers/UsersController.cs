using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Abstractions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ApiController
    {
        public UsersController(ISender sender) : base(sender) { }
    }
}
