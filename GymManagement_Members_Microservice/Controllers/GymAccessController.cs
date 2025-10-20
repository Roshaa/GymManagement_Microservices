using GymManagement_Members_Microservice.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymAccessController(ApplicationDbContext _context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(int id, CancellationToken ct = default)
        {
            //If ur confused by not getting user id, in gym every member has a card with their id that they use to access the gym
            bool access = await _context.Member.Select(m => m.Id == id && m.MemberShipActive).AnyAsync(ct);

            return Ok(access ? "Lift hard" : "You do not have an active membership");
        }

    }
}
