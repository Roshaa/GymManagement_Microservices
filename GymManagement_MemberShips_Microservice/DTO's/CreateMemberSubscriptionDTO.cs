using System.ComponentModel.DataAnnotations;

namespace GymManagement_MembersShips_Microservice.DTO_s
{
    public sealed record class CreateMemberSubscriptionDTO
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 25, ErrorMessage = "IBAN must be exactly 25 characters")]
        public string IBAN { get; set; }

        public DateOnly PaymentDay { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
        public bool DebitActive { get; set; } = true;
    }
}
