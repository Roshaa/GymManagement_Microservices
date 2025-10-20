using GymManagement_Members_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Member { get; set; }
        public DbSet<MemberDiscount> MemberDiscounts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>(e =>
            {
                e.HasIndex(x => x.Email)
                 .IsUnique()
                 .HasFilter("[Email] IS NOT NULL");

                e.HasIndex(x => x.Phone)
                 .IsUnique()
                 .HasFilter("[Phone] IS NOT NULL");

            });
        }

    }
}
