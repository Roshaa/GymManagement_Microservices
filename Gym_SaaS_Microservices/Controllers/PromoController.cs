using Gym_SaaS_Microservices.Context;
using Microsoft.AspNetCore.Mvc;

namespace Gym_SaaS_Microservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController(ApplicationDbContext _context) : ControllerBase
    {
        // GET: api/<PromoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PromoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PromoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PromoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PromoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
