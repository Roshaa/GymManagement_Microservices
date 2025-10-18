using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_SaaS_Microservices.DTO_s
{
    public record class CreatePromoDTO
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public int MonthDuration { get; set; }
    }
}
