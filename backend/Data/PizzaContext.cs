using Microsoft.EntityFrameworkCore;
using ContosoPizza.Models;

namespace ContosoPizza.Data
{
    public class PizzaContext : DbContext
    {
        public PizzaContext(DbContextOptions<PizzaContext> options)
        : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pizza> Pizzas { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<OrderPizza> OrderPizzas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            // Configure composite key for OrderPizza
            modelBuilder.Entity<OrderPizza>()
                .HasKey(op => new { op.OrderId, op.PizzaId });

            // Configure relationships
            modelBuilder.Entity<OrderPizza>()
                .HasOne(op => op.Order)
                .WithMany(o => o.Pizzas)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<PizzaTopping>()
                .HasOne(p => p.Pizza)
                .WithMany(t => t.Toppings)
                .HasForeignKey(pt => pt.PizzaId);

            modelBuilder.Entity<OrderPizza>()
                .HasOne(op => op.Pizza)
                .WithMany()
                .HasForeignKey(op => op.PizzaId);

            modelBuilder.Entity<Order>()
                .OwnsMany(o => o.Drinks);
        }
    }
}
#pragma warning restore format