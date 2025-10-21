namespace GymManagement_Members_Microservice.DTO_s
{
    public sealed record class CreateMemberSubscriptionDTO
    {
        public int MemberId { get; set; }
        public string IBAN { get; set; }
        public DateOnly PaymentDay { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public bool DebitActive { get; set; }
    }
}
