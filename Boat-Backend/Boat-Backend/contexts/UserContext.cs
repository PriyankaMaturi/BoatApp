using Microsoft.EntityFrameworkCore;
using Boat_Backend.Models;
namespace Boat_Backend.contexts
{
    public class UserContext : DbContext
    {
       public  DbSet<User> users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
             optionsBuilder.UseSqlServer("server= DESKTOP-IFVHQAF;database=BoatDb; Integrated Security=True ; MultipleActiveResultSets=true; TrustServerCertificate=true");
        }
    }
}
