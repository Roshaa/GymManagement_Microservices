using AutoMapper;
using GymManagement_Members_Microservice.Context;
using GymManagement_Members_Microservice.DTO_s;
using GymManagement_Members_Microservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GymManagement_Members_Microservice.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    //Im not going to bother in creating and injecting a service here.
    public class MemberController(ApplicationDbContext _context, IMapper _mapper) : ControllerBase
    {
        private readonly int _membersPerPage = 20;

        private int pageSkip(int page = 1)
        {
            return (page - 1) * _membersPerPage;
        }

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

            //criar tabela para verificar descontos/ PROMO
            //criar gym access history

            await _context.AddAsync(member, ct);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetMemberById), new { id = member.Id }, _mapper.Map<MemberDTO>(member));
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
    }
}
