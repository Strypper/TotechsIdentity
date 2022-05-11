using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class IdentityContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public IdentityContext(DbContextOptions options) : base(options) { }
        public DbSet<RoleLevel> RoleLevels { get; set; }
        public DbSet<Country>   Countries  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.Guid).IsUnique();
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasOne(ur => ur.Role).WithMany(r => r!.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ur => ur.User).WithMany(u => u!.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
