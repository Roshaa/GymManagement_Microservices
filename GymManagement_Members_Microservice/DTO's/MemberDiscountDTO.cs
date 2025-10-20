namespace GymManagement_Members_Microservice.DTO_s
{
    public sealed record class MemberDiscountDTO
    {

        public int MemberId { get; set; }
        public string DiscountCode { get; set; }
        public int RemainingMonths { get; set; }
        public decimal Discount { get; set; }
    }
}
