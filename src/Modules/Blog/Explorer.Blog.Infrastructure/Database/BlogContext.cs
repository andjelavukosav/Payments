using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.ToTable("ShoppingCarts", "blog"); // eksplicitno navodimo shemu
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Id).IsUnique();

            entity.HasMany(e => e.ShoppingCartItems)
                  .WithOne(i => i.ShoppingCart)
                  .HasForeignKey(i => i.ShoppingCartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShoppingCartItem>(entity =>
        {
            entity.ToTable("ShoppingCartItems", "blog"); // isto ovdje
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TourPurchaseToken>(entity =>
        {
            entity.ToTable("TourPurchaseTokens"); // obavezno plural
            entity.HasKey(e => e.Id); // nasleđeni Id
        });
    }



}