using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentDataAccessLayer.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) :
        IdentityDbContext<ApplicationUser>(options)
    {

    }
}
