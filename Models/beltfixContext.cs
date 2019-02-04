using Microsoft.EntityFrameworkCore;
 
namespace beltfix.Models
{
    public class beltfixContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public beltfixContext(DbContextOptions<beltfixContext> options) : base(options) { }
        
        public DbSet<Users> users { get; set; }
        public DbSet<Auctions> auctions { get; set; }
        public DbSet<Bidders> bidders {get; set; }
    }
}
