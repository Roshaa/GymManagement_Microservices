namespace GymManagement_Members_Microservice.DTO_s
{
    public sealed record class PromoAnswerDTO
    {
        public decimal Discount { get; set; }
        public int MonthDuration { get; set; }
    }
}
