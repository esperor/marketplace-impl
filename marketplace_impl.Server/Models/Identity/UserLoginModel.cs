using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models.Identity
{
    public class UserLoginModel
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
