using Gym_SaaS_Microservices.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_SaaS_Microservices.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Promo> Promo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Promo>(b =>
            {
                b.HasIndex(p => p.Code).IsUnique();
                b.Property(p => p.Code).IsUnicode(false);
            });

            modelBuilder.Entity<Promo>().HasData(new Promo
            {
                Id = 1,
                Code = "22ABT",
                Discount = 0.50m,
                MonthDuration = 3,
            });

            modelBuilder.Entity<Promo>().HasData(new Promo
            {
                Id = 2,
                Code = "23H5F",
                Discount = 0.20m,
                MonthDuration = 6,
            });

        }
    }
}
