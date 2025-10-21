using AutoMapper;
using AutoMapper.QueryableExtensions;
using GymManagement_MemberShips_Microservice.Context;
using GymManagement_MemberShips_Microservice.DTO_s;
using GymManagement_MemberShips_Microservice.Models;
using GymManagement_Shared_Classes.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipsController(ApplicationDbContext _context, IMapper _mapper) : ControllerBase
    {
        [HttpGet("active")]
        public async Task<IActionResult> GetMembersByDebitStatus(CancellationToken ct = default)
        {
            MemberSubscriptionDTO[] memberSubscriptions = await _context.MemberSubscriptions
                .AsNoTracking()
                .Where(m => m.DebitActive)
                .ProjectTo<MemberSubscriptionDTO>(_mapper.ConfigurationProvider)
                .ToArrayAsync(ct);

            return Ok(memberSubscriptions);
        }

        [HttpGet("{memberId:int}")]
        public async Task<IActionResult> GetMemberSubscription(int memberId, CancellationToken ct = default)
        {
            MemberSubscriptionDTO memberSubscription = await _context.MemberSubscriptions
                .AsNoTracking()
                .Where(m => m.MemberId == memberId)
                .ProjectTo<MemberSubscriptionDTO>(_mapper.ConfigurationProvider)
                .SingleAsync(ct);

            if (memberSubscription == null) return NotFound();

            return Ok(memberSubscription);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMemberSubscription([FromBody] CreateMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            MemberSubscription memberSubscription = _mapper.Map<MemberSubscription>(dto);

            if (await _context.MemberSubscriptions.AnyAsync(m => m.MemberId == dto.MemberId, ct)) return Conflict($"Member with ID {dto.MemberId} already has a subscription.");

            await _context.MemberSubscriptions.AddAsync(memberSubscription);
            await _context.SaveChangesAsync(ct);

            MemberSubscriptionDTO respDTO = _mapper.Map<MemberSubscriptionDTO>(memberSubscription);

            return CreatedAtAction(nameof(GetMemberSubscription), new { memberId = memberSubscription.MemberId }, respDTO);

        }

        [HttpPut("change_debit")]
        public async Task<IActionResult> ChangeMemberSubscription([FromBody] ChangeMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            MemberSubscription memberSubscription = await _context.MemberSubscriptions
                .Where(m => m.MemberId == dto.MemberId)
                .FirstOrDefaultAsync(ct);

            if (memberSubscription == null) return NotFound();

            memberSubscription.DebitActive = dto.DebitActive;

            await _context.SaveChangesAsync(ct);

            MemberSubscriptionDTO memberSubscriptionDTO = _mapper.Map<MemberSubscriptionDTO>(memberSubscription);

            return Ok(memberSubscriptionDTO);
        }

    }
}
