using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace My_personal_budget_web_api.Context
{
    public class DataBaseContextFactory : IDesignTimeDbContextFactory<DataBaseContext>
    {
        public DataBaseContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("TestConnection");
            if (connectionString == null)
            {
                throw new Exception("no db connection string");
            }
            var optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
            optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            return new DataBaseContext(optionsBuilder.Options);
        }
    }
}
