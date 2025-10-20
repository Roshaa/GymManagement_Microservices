using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagement_Members_Microservice.Models
{
    public class MemberDiscount
    {
        //Only 1 discount can be active, so 1:1 relation
        [Key, ForeignKey(nameof(Member))]
        public int MemberId { get; set; }
        public string DiscountCode { get; set; }
        public int RemainingMonths { get; set; }
        public decimal Discount { get; set; }

        //Keys
        public Member Member { get; set; }
    }
}
