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
    public DbSet<PlanCell> PlanCells { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<UserType> UserTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Cấu hình mối quan hệ nhiều-nhiều giữa User và Plan thông qua UserPlan
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

      // Cấu hình mối quan hệ một-nhiều giữa User và UserType
      modelBuilder.Entity<User>()
          .HasOne(u => u.UserType)
          .WithMany(ut => ut.Users)
          .HasForeignKey(u => u.UserTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(true);

      // Cấu hình mối quan hệ một-nhiều giữa Plan và PlanCell
      modelBuilder.Entity<PlanCell>()
          .HasOne(pc => pc.Plan)
          .WithMany(p => p.PlanCells)
          .HasForeignKey(pc => pc.PlanId)
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
