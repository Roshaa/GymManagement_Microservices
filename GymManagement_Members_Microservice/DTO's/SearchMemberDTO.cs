namespace GymManagement_Members_Microservice.DTO_s
{
    public sealed record class SearchMemberDTO
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IBAN { get; set; }
    }
}
