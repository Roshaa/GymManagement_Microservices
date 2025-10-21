using GymManagement_MemberShips_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MemberSubscription> MemberSubscriptions { get; set; }

    }
}
