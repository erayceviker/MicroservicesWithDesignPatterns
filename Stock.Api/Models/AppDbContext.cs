using Microsoft.EntityFrameworkCore;

namespace Stock.Api.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) :DbContext(options)
    {
        public DbSet<Stock> Stocks { get; set; }
    }
}
