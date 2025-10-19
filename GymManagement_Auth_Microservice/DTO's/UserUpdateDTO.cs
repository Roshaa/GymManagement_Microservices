using System.ComponentModel.DataAnnotations;

namespace GymManagement_Auth_Microservice.DTO_s
{
    public class UserUpdateDTO
    {
        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Invalid Name")]
        public string Name { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }
    }
}
