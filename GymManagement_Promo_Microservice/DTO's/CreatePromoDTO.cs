using System.ComponentModel.DataAnnotations;

namespace GymManagement_Promo_Microservice.DTO_s
{
    public sealed record class CreatePromoDTO
    {
        [Required, StringLength(5, MinimumLength = 5)]
        [RegularExpression(@"^[A-Z0-9]{5}$")]
        public string Code { get; init; } = null!;

        [Required, Range(0.1, 1.0)]
        public decimal Discount { get; init; }

        [Required, Range(1, 6)]
        public int MonthDuration { get; init; }
    }
}
