using GymManagement_Members_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Member { get; set; }

    }
}
