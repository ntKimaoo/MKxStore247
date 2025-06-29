using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Models;
using System.Reflection.Emit;

namespace MKxStore247.Data;

public class MKxStore247Context : IdentityDbContext<UserApplication>
{
    public MKxStore247Context(DbContextOptions<MKxStore247Context> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        base.OnModelCreating(builder);
        builder.Entity<UserApplication>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(200);
            entity.Property(u => u.CreatedBy).HasMaxLength(256);
            entity.Property(u => u.Address).HasMaxLength(500);
        });
        builder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
            entity.Property(n => n.Message).HasMaxLength(1000);
            entity.Property(n => n.Link).HasMaxLength(500);
            entity.Property(n => n.CreatedBy).HasMaxLength(256);
        });
        builder.Entity<Address>(entity =>
        {
            entity.Property(a => a.City).HasMaxLength(100);
            entity.Property(a => a.Country).HasMaxLength(100);
        });
        builder.Entity<Shop>(entity =>
        {
            entity.HasOne(s => s.Owner)
                .WithMany(u => u.Shops)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Product>(entity =>
        {
            entity.HasOne(p => p.Shop)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(p => p.ProductName).HasMaxLength(100).IsRequired();
        });
        builder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.CartId);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            entity.Property(c => c.CreatedBy).HasMaxLength(256);
            entity.Property(c => c.UserId).IsRequired();
        });
        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.CartItemId);

            entity.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(ci => ci.Quantity).IsRequired();
        });
        builder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(pi => pi.ImageId);
            entity.Property(pi => pi.ImageUrl).HasMaxLength(255).IsRequired();
            entity.HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });


    }
    public DbSet<UserApplication> UserApplications { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Shop> Shop { get; set; }
    public DbSet<CategoryProduct> CategoryProduct { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<StockImport> StockImport { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<OrderDetail> OrderDetail { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<PaymentMethod> PaymentMethod { get; set; }
    public DbSet<UserCoupon> UserCoupon { get; set; }
    public DbSet<Coupon> Coupon { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }



}
