using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Cấu hình mối quan hệ một-nhiều từ Category đến Product
      modelBuilder.Entity<Category>()
          .HasMany(c => c.Products)
          .WithOne(p => p.Category)
          .HasForeignKey(p => p.CategoryId);
    }
  }
}
