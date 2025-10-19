namespace GymManagement_Promo_Microservice.DTO_s
{
    public record class PromoDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public int MonthDuration { get; set; }
    }
}
