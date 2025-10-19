namespace GymManagement_Auth_Microservice.DTO_s
{
    public sealed record class UpdateUserRolesDTO
    {
        public string UserId { get; set; } = default!;
        public string[] Added { get; set; } = Array.Empty<string>();
        public string[] Removed { get; set; } = Array.Empty<string>();
        public string[] CurrentRoles { get; set; } = Array.Empty<string>();
    }
}
