
namespace GymManagement_Members_Microservice.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IBAN { get; set; }
        public DateOnly RegisterDay { get; set; }
        public DateOnly ActiveUntilDay { get; set; }

        //Keys
        // 1:1, Member can have a discount active on his membership
        public MemberDiscount? MemberDiscount { get; set; }
    }
}
