namespace GymManagement_Members_Microservice.DTO_s
{
    public sealed record class MemberDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IBAN { get; set; }
        public DateOnly RegisterDay { get; set; }
        public DateOnly ActiveUntilDay { get; set; }
        public bool DebitActive { get; set; }
    }
}
