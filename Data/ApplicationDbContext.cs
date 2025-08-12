using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;

namespace AspnetCoreMvcFull.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<UserType> UserTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<UserPlan>()
          .HasKey(up => new { up.UserId, up.PlanId });

      modelBuilder.Entity<UserPlan>()
          .HasOne(up => up.User)
          .WithMany(u => u.UserPlans)
          .HasForeignKey(up => up.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<UserPlan>()
          .HasOne(up => up.Plan)
          .WithMany(p => p.UserPlans)
          .HasForeignKey(up => up.PlanId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<User>()
          .HasOne(u => u.UserType)
          .WithMany(ut => ut.Users)
          .HasForeignKey(u => u.UserTypeId)
          .OnDelete(DeleteBehavior.Restrict) // Ngăn xóa UserType nếu có User liên quan
          .IsRequired(true); // Bắt buộc UserTypeId
    }
  }
}
