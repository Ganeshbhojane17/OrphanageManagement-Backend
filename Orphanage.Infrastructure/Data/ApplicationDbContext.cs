using Microsoft.EntityFrameworkCore;
using Orphanage.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orphanage.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.Email).IsUnique();
            });
            builder.Entity<Role>(b => { b.HasKey(r => r.Id); });
            builder.Entity<UserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });
                b.HasOne(ur => ur.User).WithMany(u =>
                u.UserRoles).HasForeignKey(ur => ur.UserId);
                b.HasOne(ur => ur.Role).WithMany(r =>
                r.UserRoles).HasForeignKey(ur => ur.RoleId);
            });
            builder.Entity<RefreshToken>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasOne(r => r.User).WithMany(u =>
                u.RefreshTokens).HasForeignKey(r => r.UserId);
            });

        }
    }
}
