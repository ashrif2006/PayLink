using Microsoft.EntityFrameworkCore;

namespace PayLink.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> optoins) : base(optoins)
        {
        }
    }
}
