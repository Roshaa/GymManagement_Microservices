namespace GymManagement_Auth_Microservice.DTO_s
{
    namespace GymManagement_Auth_Microservice.DTO_s
    {
        public sealed record class UserDetailsDTO
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public bool LockoutEnabled { get; set; }
            public System.DateTimeOffset? LockoutEnd { get; set; }
            public int AccessFailedCount { get; set; }
        }
    }
}