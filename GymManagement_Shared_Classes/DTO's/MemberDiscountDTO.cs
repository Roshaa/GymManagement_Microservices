namespace GymManagement_Shared_Classes.DTO_s
{
    public sealed record class MemberDiscountDTO
    {

        public int MemberId { get; set; }
        public string DiscountCode { get; set; }
        public int RemainingMonths { get; set; }
        public decimal Discount { get; set; }
    }
}
