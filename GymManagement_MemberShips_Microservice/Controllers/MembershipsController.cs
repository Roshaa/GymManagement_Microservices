using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement_MemberShips_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipsController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateMemberSubscription()
        {
            return Ok();
        }


    }
}
