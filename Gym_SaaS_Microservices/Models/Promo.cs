using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_SaaS_Microservices.Models
{
    public class Promo
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(5)]
        [RegularExpression(@"^[A-Z0-9]{5}$")]
        public string Code { get; set; }

        [Required]
        [Range(0.1, 1.0, ErrorMessage = "Discount must be between 0.1 and 1.0")]
        [Column(TypeName = "decimal(3, 2)")]
        public decimal Discount { get; set; } //0.1 = 10%

        [Required]
        [Range(1, 6, ErrorMessage = "Month Duration must be at least 1 and atmost 6")]
        public int MonthDuration { get; set; } //Duration of months the gym subscription promo should last 
    }
}
