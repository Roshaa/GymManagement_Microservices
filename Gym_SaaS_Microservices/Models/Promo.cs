using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagement_Promo_Microservice.Models
{
    public class Promo
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal Discount { get; set; } // 0.1 = 10%
        public int MonthDuration{  get; set; }
    }
}
