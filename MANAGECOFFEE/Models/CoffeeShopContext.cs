using MANAGECOFFEE.Models;
using ManagementCoffee.Models;
using Microsoft.EntityFrameworkCore;

namespace MANAGECOFFEE.Data
{
    public class CoffeeShopContext : DbContext
    {
        public CoffeeShopContext(DbContextOptions options) : base(options) { }
        public DbSet<Coffee> Coffee { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Statistics> Statistic { get; set; }
        public DbSet<Invoice> Invoice { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItem");
                entity.HasKey(e => new { e.OrderId, e.CoffeeId });

            });
            
        }
    }
}
