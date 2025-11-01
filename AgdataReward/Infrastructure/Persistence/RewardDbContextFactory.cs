using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Design-time factory used by EF tools (Add-Migration / Update-Database) when the startup project isn't available.
    /// Adjust the connection string if needed.
    /// </summary>
    public class RewardDbContextFactory : IDesignTimeDbContextFactory<RewardDbContext>
    {
        public RewardDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RewardDbContext>();

            // Change this connection string if your SQL Server instance differs.
            var conn = "Server=SANKALP-26\\SQLEXPRESS;Database=AgdataRewardDB;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(conn, sql =>
            {
                // optional: specify migrations assembly explicitly if needed
                sql.MigrationsAssembly(typeof(RewardDbContext).Assembly.FullName);
            });

            return new RewardDbContext(optionsBuilder.Options);
        }
    }
}
