using AppsServer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppsServer.Models.Context
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<MasterProduct> MasterProducts { get; set; }
    }
}
