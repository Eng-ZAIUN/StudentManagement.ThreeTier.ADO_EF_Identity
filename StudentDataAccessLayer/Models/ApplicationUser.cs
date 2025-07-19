using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentDataAccessLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(60)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(60)]
        public string LastName { get; set; } = string.Empty;
    }
}
