namespace GymManagement_Shared_Classes.DTO_s
{
    public sealed record class ChangeMemberSubscriptionDTO
    {
        public int MemberId { get; set; }
        public bool DebitActive { get; set; }
    }
}
