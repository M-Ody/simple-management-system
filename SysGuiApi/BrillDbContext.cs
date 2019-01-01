using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SysGuiApi.Models;

namespace SysGuiApi
{
    public partial class BrillDbContext : DbContext
    {
        public BrillDbContext()
        {
        }

        public BrillDbContext(DbContextOptions<BrillDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PermissionGroup> PermissionGroup { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<City> City { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Database=BrillDb;Username=postgres;Password=BasketCase94");
            }
        }
    }
}
