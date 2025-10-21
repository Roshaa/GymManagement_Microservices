namespace GymManagement_MemberShips_Microservice.DTO_s
{
    public sealed record class MemberSubscriptionDTO
    {
        public int MemberId { get; set; }
        public string IBAN { get; set; }
        public DateOnly PaymentDay { get; set; }
        public bool DebitActive { get; set; }
    }
}
