namespace GymManagement_Shared_Classes.Events
{
    public sealed record class PaymentMessage(int MemberId, bool PaymentSuccessful, DateOnly? NextPayment);
}
