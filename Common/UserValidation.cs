using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserValidation
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username cannot be blank")]
        [StringLength(50, ErrorMessage = "Username must be shorter than 50 characters")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be blank")]
        [StringLength(15, ErrorMessage = "Password must be longer than 6 characters and shorter than 15 characters", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$", ErrorMessage = "Passwords must contain at least one digit, one uppercase and one lowercase")]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name cannot be blank")]
        [RegularExpression(@"^[a-zA-Z\s]+$",ErrorMessage = "First name must contain only letters of the alphabet")]
        public string Name { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name cannot be blank")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name must contain only letters of the alphabet")]
        public string Surname { get; set; }
    }

    [MetadataType(typeof(UserValidation))]
    public partial class User
    {

    }
}
