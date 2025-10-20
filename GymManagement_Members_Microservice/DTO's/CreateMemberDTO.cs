using System.ComponentModel.DataAnnotations;

namespace GymManagement_Members_Microservice.DTO_s
{
    public class CreateMemberDTO
    {
        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Invalid Name")]
        public string Name { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 25, ErrorMessage = "IBAN must be exactly 25 characters")]
        public string IBAN { get; set; }

        public DateTime RegisterDay { get; set; } = DateTime.Now;

        public bool DebitActive { get; set; }
    }
}
