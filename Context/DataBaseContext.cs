using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Context
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }
    }
}
