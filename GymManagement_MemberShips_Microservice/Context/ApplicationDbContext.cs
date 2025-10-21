using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



    }
}
