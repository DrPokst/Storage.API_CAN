using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Storage.API.Models;
using Storage.API_CAN.Models;

namespace Storage.API.Data
{
    public class DataContext : IdentityDbContext<User, AppRole, int, IdentityUserClaim<int>, UserRole, 
    IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Componentas> Componentass { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Photo2> Photos2 { get; set; }
        public DbSet<UserPhoto> UserPhoto { get; set; }
        public DbSet<Reel> Reels { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<BomList> BomList { get; set; }
        public DbSet<BomName> BomName { get; set; }
        public DbSet<PnP> PnP { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });
        }
        internal object ToList()
        {
            throw new NotImplementedException();
        }
    }
}