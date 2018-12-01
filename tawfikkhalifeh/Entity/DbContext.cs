using System.Data.Entity;

namespace tawfikkhalifeh.Entity
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }
        public DbSet<Contact> Contact { get; set; }
    }
}
