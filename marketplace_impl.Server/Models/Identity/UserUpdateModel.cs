using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models.Identity
{
    public class UserUpdateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}
