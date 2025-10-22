namespace GymManagement_Shared_Classes.Events
{
    public sealed record class PaymentMessage
    {
        public int MemberId { get; set; }
        public bool PaymentSuccessful { get; set; }
        public DateOnly NextPayment { get; set; }
    }
}
