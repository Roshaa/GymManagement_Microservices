using GymManagement_Promo_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Promo_Microservice.Context
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
                b.Property(p => p.Code).HasMaxLength(5).IsUnicode(false);
                b.Property(p => p.Discount).HasPrecision(3, 2);
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
