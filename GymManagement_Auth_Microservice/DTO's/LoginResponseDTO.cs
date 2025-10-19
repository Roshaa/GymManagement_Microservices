namespace GymManagement_Auth_Microservice.DTO_s
{
    public sealed record class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
