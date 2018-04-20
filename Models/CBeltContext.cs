using Microsoft.EntityFrameworkCore;
 
namespace C_SharpBelt.Models
{
    public class CBeltContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public CBeltContext(DbContextOptions<CBeltContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Activitie> activities { get; set; }
        public DbSet<Participant> participants { get; set; }
    }
}