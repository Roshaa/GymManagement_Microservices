using AutoMapper;
using GymManagement_MemberShips_Microservice.Context;
using GymManagement_MemberShips_Microservice.DTO_s;
using GymManagement_MemberShips_Microservice.Models;
using GymManagement_MembersShips_Microservice.DTO_s;
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
                .AsNoTracking().Where(m => m.DebitActive).Select(m => new MemberSubscriptionDTO
                {
                    MemberId = m.MemberId,
                    IBAN = m.IBAN,
                    PaymentDay = m.PaymentDay,
                    DebitActive = m.DebitActive
                }).ToArrayAsync(ct);

            return Ok(memberSubscriptions);
        }

        [HttpGet("{memberId:int}")]
        public async Task<IActionResult> GetMemberSubscription(int memberId, CancellationToken ct = default)
        {
            MemberSubscriptionDTO memberSubscription = await _context.MemberSubscriptions
                .AsNoTracking().Where(m => m.MemberId == memberId).Select(m => new MemberSubscriptionDTO
                {
                    MemberId = m.MemberId,
                    IBAN = m.IBAN,
                    PaymentDay = m.PaymentDay,
                    DebitActive = m.DebitActive
                }).SingleAsync(ct);

            if (memberSubscription == null) return NotFound();

            return Ok(memberSubscription);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMemberSubscription([FromBody] CreateMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            MemberSubscription memberSubscription = _mapper.Map<MemberSubscription>(dto);

            await _context.MemberSubscriptions.AddAsync(memberSubscription);
            await _context.SaveChangesAsync(ct);

            MemberSubscriptionDTO respDTO = _mapper.Map<MemberSubscriptionDTO>(memberSubscription);

            return CreatedAtAction(nameof(GetMemberSubscription), new { id = memberSubscription.Id }, respDTO);

        }

        [HttpPut("change_debit")]
        public async Task<IActionResult> ChangeMemberSubscription([FromBody] ChangeMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            MemberSubscription memberSubscription = await _context.MemberSubscriptions
                .Where(m => m.MemberId == dto.MemberId).FirstOrDefaultAsync(ct);

            if (memberSubscription == null) return NotFound();

            memberSubscription.DebitActive = dto.DebitActive;

            await _context.SaveChangesAsync(ct);

            MemberSubscriptionDTO memberSubscriptionDTO = _mapper.Map<MemberSubscriptionDTO>(memberSubscription);

            return Ok(memberSubscriptionDTO);
        }

    }
}
