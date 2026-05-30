using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models.Identity
{
    public class UserRegisterModel : UserLoginModel
    {
        [Required]
        public string Name { get; set; }
    }
}
