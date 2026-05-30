using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace course.Server.Data
{
    [Table("users")]
    public class ApplicationUser
    {
        [Key]
        [PersonalData]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Phone]
        [ProtectedPersonalData]
        public required string Phone { get; set; }

        public string? PasswordHash { get; set; }

        public ApplicationUser() { }

        [SetsRequiredMembers]
        public ApplicationUser(ApplicationUser other)
        {
            Id = other.Id;
            Name = other.Name;
            Phone = other.Phone;
            PasswordHash = other.PasswordHash;
        }
    }
}
