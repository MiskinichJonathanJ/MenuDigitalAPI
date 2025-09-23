using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Dish> Dish { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<DeliveryType> DeliveryType { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Status> Status  { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.HasKey(d => d.DishId);
                entity.Property(d => d.DishId)
                    .HasColumnType("uuid");
                entity.Property(d => d.Name).HasColumnType("varchar(255)");
                entity.Property(d => d.Price).HasColumnType("decimal");
                entity.Property(d => d.Description).HasColumnType("text");
                entity.Property(d => d.ImageUrl).HasColumnType("text");

                entity.HasOne(d => d.CategoryNav)
                    .WithMany(c => c.Dishes)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Name).HasColumnType("varchar(25)");
                entity.Property(c => c.Description).HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<DeliveryType>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).ValueGeneratedOnAdd();
                entity.Property(d => d.Name).HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderId).ValueGeneratedOnAdd();
                entity.Property(o => o.DeliveryTo).HasColumnType("varchar(255)");
                entity.Property(o => o.Notes).HasColumnType("text");
                entity.Property(d => d.Price).HasColumnType("decimal");

                entity.HasOne(o =>o.DeliveryTypeNav)
                        .WithMany(d => d.OrdersNav)
                        .HasForeignKey(o => o.DeliveryType)
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                entity.HasOne(o => o.StatusNav)
                    .WithMany(s => s.OrdersNav)
                    .HasForeignKey(d => d.OverallStatus)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(o => o.OrderItemId);
                entity.Property(o => o.OrderItemId).ValueGeneratedOnAdd();
                entity.Property(o => o.Notes).HasColumnType("text");

                entity.HasOne(o => o.DishNav)
                        .WithMany()
                        .HasForeignKey(o => o.Dish)
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                entity.HasOne(o => o.OrderNav)
                    .WithMany(ord => ord.Items)
                    .HasForeignKey(o => o.Order)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(o => o.StatusNav)
                    .WithMany(s => s.OrderItemsNav)
                    .HasForeignKey(o => o.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Name).HasColumnType("varchar(25)");
            });
        }
    }
}
