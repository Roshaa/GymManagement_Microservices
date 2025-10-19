namespace GymManagement_Auth_Microservice.DTO_s
{
    public sealed record class LoginUserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
