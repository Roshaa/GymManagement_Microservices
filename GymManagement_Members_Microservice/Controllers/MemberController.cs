using AutoMapper;
using GymManagement_Members_Microservice.Client;
using GymManagement_Members_Microservice.Context;
using GymManagement_Members_Microservice.DTO_s;
using GymManagement_Members_Microservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    //Im not going to bother in creating and injecting a service here for a personal project...
    public class MemberController(ApplicationDbContext _context, IMapper _mapper, PromoClient _promoClient, MemberShipClient _membershipClient) : ControllerBase
    {
        private readonly int _membersPerPage = 20;

        private int pageSkip(int page = 1)
        {
            return (page - 1) * _membersPerPage;
        }

        #region Members
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestMembers(int page = 1, CancellationToken ct = default)
        {
            int skip = pageSkip(page);

            MemberDTO[] members = await _context.Member.AsNoTracking().OrderByDescending(m => m.Id)
                .Select(m => new MemberDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Phone = m.Phone,
                    Email = m.Email,
                    IBAN = m.IBAN,
                    RegisterDay = m.RegisterDay,
                    DebitActive = m.DebitActive,
                    MemberShipActive = m.MemberShipActive
                })
                .Skip(skip)
                .Take(_membersPerPage)
                .ToArrayAsync(ct);

            return Ok(members);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchMembers([FromBody] SearchMemberDTO dto, int page = 1, CancellationToken ct = default)
        {
            int skip = pageSkip(page);

            IQueryable<MemberDTO> query = _context.Member.AsNoTracking().Select(m => new MemberDTO
            {
                Id = m.Id,
                Name = m.Name,
                Phone = m.Phone,
                Email = m.Email,
                IBAN = m.IBAN,
                RegisterDay = m.RegisterDay,
                DebitActive = m.DebitActive,
                MemberShipActive = m.MemberShipActive
            });

            if (!string.IsNullOrWhiteSpace(dto.Name))
                query = query.Where(m => m.Name.ToLower().Contains(dto.Name.ToLower()));

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                query = query.Where(m => m.Phone.ToLower().Contains(dto.Phone.ToLower()));

            if (!string.IsNullOrWhiteSpace(dto.Email))
                query = query.Where(m => m.Email.ToLower().Contains(dto.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(dto.IBAN))
                query = query.Where(m => m.IBAN.ToLower().Contains(dto.IBAN.ToLower()));

            MemberDTO[] members = await query
                .OrderBy(m => m.Name)
                .Skip(skip)
                .Take(_membersPerPage)
                .ToArrayAsync(ct);

            return Ok(members);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMemberById(int id, CancellationToken ct = default)
        {
            Member? member = await _context.Member.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

            if (member == null) return NotFound();

            return Ok(_mapper.Map<MemberDTO>(member));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] CreateMemberDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Member member = _mapper.Map<Member>(dto);

            await _context.AddAsync(member, ct);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict("A member with the same email or phone already exists.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && sql.Number == 547)
            {
                return BadRequest("Invalid request on creating member");
            }

            await CreateMemberDiscount(new CreateMemberDiscountDTO
            {
                Code = dto.Code,
                MemberId = member.Id
            }, ct);

            //Lets assume that in real world the first payment is in a automatic payment terminal (tpa)
            //Issue a payment to memberships creation via http client
            var createMemberSubscriptionDTO = _mapper.Map<CreateMemberSubscriptionDTO>(member);
            bool subscriptionCreated = await _membershipClient.CreateSubscriptionPayment(createMemberSubscriptionDTO);
            string subscriptionMessage = subscriptionCreated ? "Membership subscription payment created successfully." : "Could not create membership subscription payment.";

            return CreatedAtAction(nameof(GetMemberById), new { id = member.Id, subscriptionMessage }, _mapper.Map<MemberDTO>(member));
        }

        [HttpPost("create_payment_subscription/{memberId:int}")]
        public async Task<IActionResult> CreateMemberPaymentSubscription(int memberId, CancellationToken ct = default)
        {
            Member? member = await _context.Member.AsNoTracking().FirstOrDefaultAsync(m => m.Id == memberId, ct);

            if (member == null) return NotFound();

            var createMemberSubscriptionDTO = _mapper.Map<CreateMemberSubscriptionDTO>(member);

            bool subscriptionCreated = await _membershipClient.CreateSubscriptionPayment(createMemberSubscriptionDTO);

            if (!subscriptionCreated) return BadRequest("Could not create subscription payment for member");

            return Ok();
        }

        //In real world scenarios its more complex than this, cause the debit can be cancelled from bank side too and user can also pay manually
        //For the sake of this personal project we will keep it simple
        [HttpPut("debit_status/{memberId:int}")]
        public async Task<IActionResult> UpdateMemberDebitStatus(int memberId, CancellationToken ct = default)
        {
            Member? member = await _context.Member.FirstOrDefaultAsync(m => m.Id == memberId, ct);
            if (member == null) return NotFound();

            ChangeMemberSubscriptionDTO changeMemberSubscriptionDTO = _mapper.Map<ChangeMemberSubscriptionDTO>(member);

            bool sucessful = await _membershipClient.ChangeMemberDebitStatus(changeMemberSubscriptionDTO, ct);

            if (!sucessful) return BadRequest("Could not update member debit status in the memberships service, please verify");

            member.DebitActive = !member.DebitActive;
            await _context.SaveChangesAsync(ct);

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] UpdateMemberDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Member? member = await _context.Member.FirstOrDefaultAsync(m => m.Id == id, ct);

            if (member == null) return NotFound();

            member.Name = dto.Name;
            member.Phone = dto.Phone;
            member.Email = dto.Email;
            member.IBAN = dto.IBAN;

            await _context.SaveChangesAsync(ct);

            return Ok(_mapper.Map<MemberDTO>(member));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMember(int id, CancellationToken ct = default)
        {
            await _context.Member.Where(m => m.Id == id).ExecuteDeleteAsync(ct);

            return NoContent();
        }
        #endregion

        //Should be in a separate controller in real world scenario
        #region Discounts
        [HttpPost("member_discount")]
        public async Task<IActionResult> CreateDiscountForMember([FromBody] CreateMemberDiscountDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Member? member = await _context.Member.AsNoTracking().FirstOrDefaultAsync(m => m.Id == dto.MemberId, ct);

            if (member == null) return NotFound();

            bool created = await CreateMemberDiscount(dto, ct);

            if (!created) return BadRequest("Invalid discount code, check if member already has an active discount");

            return Ok();
        }

        [HttpGet("{memberId:int}/discount")]
        public async Task<IActionResult> GetMemberDiscount(int memberId, CancellationToken ct = default)
        {
            MemberDiscountDTO dto = await _context.MemberDiscounts.AsNoTracking()
                .Select(md => new MemberDiscountDTO
                {
                    MemberId = md.MemberId,
                    DiscountCode = md.DiscountCode,
                    Discount = md.Discount,
                    RemainingMonths = md.RemainingMonths
                })
                .FirstOrDefaultAsync(d => d.MemberId == memberId, ct);

            if (dto == null) return NotFound();

            return Ok(dto);
        }

        [HttpDelete("member_discount/{memberId:int}")]
        public async Task<IActionResult> DeleteMemberDiscount(int memberId, CancellationToken ct = default)
        {
            await _context.MemberDiscounts.Where(md => md.MemberId == memberId).ExecuteDeleteAsync(ct);
            return NoContent();
        }

        //In real world this should be in a service
        private async Task<PromoAnswerDTO> GetCodeDiscount(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            PromoAnswerDTO promo = await _promoClient.GetDiscountByCodeAsync(code, ct);

            return promo;
        }

        //In real world this should be in a service
        private async Task<bool> CreateMemberDiscount(CreateMemberDiscountDTO dto, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                PromoAnswerDTO promo = await GetCodeDiscount(dto.Code);

                if (promo != null)
                {
                    if (await _context.MemberDiscounts.AnyAsync(d => d.MemberId == dto.MemberId)) return false;

                    MemberDiscount memberDiscount = new MemberDiscount();

                    memberDiscount.MemberId = dto.MemberId;
                    memberDiscount.DiscountCode = dto.Code;
                    memberDiscount.Discount = promo.Discount;
                    memberDiscount.RemainingMonths = promo.MonthDuration;

                    await _context.AddAsync(memberDiscount);
                    await _context.SaveChangesAsync(ct);
                    return true;
                }

            }
            return false;
        }
        #endregion
    }
}
