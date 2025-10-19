using System.ComponentModel.DataAnnotations;

namespace GymManagement_Auth_Microservice.DTO_s
{
    public sealed record class RegisterUserDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Invalid Name")]
        public string Name { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
    }
}
