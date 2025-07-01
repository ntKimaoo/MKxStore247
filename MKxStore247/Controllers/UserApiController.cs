using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MKxStore247.Models;

namespace MKxStore247.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<UserApplication> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

       
    }
}
