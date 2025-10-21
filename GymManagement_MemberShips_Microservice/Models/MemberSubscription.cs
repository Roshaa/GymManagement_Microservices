using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Models
{
    [Index(nameof(MemberId), IsUnique = true)]
    public class MemberSubscription
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string IBAN { get; set; }
        public DateOnly PaymentDay { get; set; }
        public bool DebitActive { get; set; }
    }
}
