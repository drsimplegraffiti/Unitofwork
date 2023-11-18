
using Microsoft.EntityFrameworkCore;
using PocketBook.Domain;

namespace PocketBook.Infrastructure
{
   public class ApplicationDbContext: DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
}