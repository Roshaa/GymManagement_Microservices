using AutoMapper;
using Gym_SaaS_Microservices.Context;
using Gym_SaaS_Microservices.DTO_s;
using Gym_SaaS_Microservices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_SaaS_Microservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController(ApplicationDbContext _context, IMapper _mapper) : ControllerBase
    {
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
            PromoDTO promoDto = await _context.Promo.AsNoTracking().Select(p=> new PromoDTO
            {
                Id = p.Id,
                Code = p.Code,
                Discount = p.Discount,
                MonthDuration = p.MonthDuration
            }) 
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
