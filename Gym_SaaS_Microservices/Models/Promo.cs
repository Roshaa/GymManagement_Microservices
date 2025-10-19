using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_SaaS_Microservices.Models
{
    public class Promo
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal Discount { get; set; } // 0.1 = 10%
        public int MonthDuration{  get; set; }
    }
}
