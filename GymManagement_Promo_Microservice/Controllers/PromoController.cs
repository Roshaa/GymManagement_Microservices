using AutoMapper;
using AutoMapper.QueryableExtensions;
using GymManagement_Promo_Microservice.Context;
using GymManagement_Promo_Microservice.DTO_s;
using GymManagement_Promo_Microservice.Models;
using GymManagement_Shared_Classes.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Promo_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PromoController(ApplicationDbContext _context, IMapper _mapper) : ControllerBase
    {
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetPromoByCode(string code, CancellationToken ct = default)
        {
            PromoAnswerDTO promo = await _context.Promo
                .AsNoTracking()
                .Where(p => p.Code == code)
                .ProjectTo<PromoAnswerDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            if (promo == null) return NotFound();

            return Ok(promo);
        }

        [HttpGet]
        public async Task<IActionResult> GetPromos(int page = 1, CancellationToken ct = default)
        {
            int elementsPerPage = 50;
            int skipElements = (page - 1) * elementsPerPage;

            PromoDTO[] promoDtoList = await _context.Promo.AsNoTracking().OrderBy(p => p.Id).Select(p => new PromoDTO
            {
                Id = p.Id,
                Code = p.Code,
                Discount = p.Discount,
                MonthDuration = p.MonthDuration
            })
                .Skip(skipElements)
                .Take(elementsPerPage)
                .ToArrayAsync(ct);

            return Ok(promoDtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken ct = default)
        {
            PromoDTO promoDto = await _context.Promo
                .AsNoTracking()
                .ProjectTo<PromoDTO>(_mapper.ConfigurationProvider)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);

            if (promoDto == null) return NotFound();

            return Ok(promoDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromo([FromBody] CreatePromoDTO dto, CancellationToken ct = default)
        {
            if (await _context.Promo.AsNoTracking().AnyAsync(p => p.Code == dto.Code, ct))
                return Conflict("Code already exists");

            Promo promo = _mapper.Map<Promo>(dto);
            await _context.AddAsync(promo);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return Conflict("Code already exists");
            }

            PromoDTO responseDTO = _mapper.Map<PromoDTO>(promo);

            return CreatedAtAction(nameof(Get), new { id = promo.Id }, responseDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromo(int id, CancellationToken ct = default)
        {
            int affectedRows = await _context.Promo.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
            if (affectedRows == 0) return NotFound();

            return NoContent();
        }


    }
}
