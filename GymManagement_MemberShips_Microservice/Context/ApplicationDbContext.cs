using GymManagement_MemberShips_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MemberSubscription> MemberSubscriptions { get; set; }
        public DbSet<PaymentHistory> PaymentHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MemberSubscription>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasAlternateKey(x => x.MemberId).HasName("AK_MemberSubscription_MemberId");
            });

            modelBuilder.Entity<PaymentHistory>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.MemberSubscription)
                 .WithMany(ms => ms.PaymentHistories)
                 .HasForeignKey(x => x.MemberId)
                 .HasPrincipalKey(ms => ms.MemberId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.PaymentDate)
                 .HasColumnType("date")
                 .HasDefaultValueSql("CAST(GETDATE() AS date)");
            });
        }

    }
}
