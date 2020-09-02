using System;
using Microsoft.EntityFrameworkCore;
using Storage.API.Models;

namespace Storage.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Value> Values { get; set; }
        public DbSet<Componentas> Componentass { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Photo2> Photos2 { get; set; }
        public DbSet<Reel> Reels { get; set; }
        public DbSet<History> History { get; set; }

        internal object ToList()
        {
            throw new NotImplementedException();
        }
    }
}