namespace GymManagement_MemberShips_Microservice.Models
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string IBAN { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal Amount { get; set; }


        public MemberSubscription? MemberSubscription { get; set; }
    }
}
