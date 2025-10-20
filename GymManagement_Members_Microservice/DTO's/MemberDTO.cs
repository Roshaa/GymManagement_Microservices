namespace GymManagement_Members_Microservice.DTO_s
{
    public class MemberDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IBAN { get; set; }
        public DateTime RegisterDay { get; set; } = DateTime.Now;
        public bool DebitActive { get; set; }
        public bool MemberShipActive { get; set; }
    }
}
