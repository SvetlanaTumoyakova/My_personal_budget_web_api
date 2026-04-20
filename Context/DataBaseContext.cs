using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Context
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи один-ко-многим: User → Accounts
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи один-ко-многим: Account → AccountType
            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountType)
                .WithMany(at => at.Accounts)
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Заполнение данных для AccountType
            modelBuilder.Entity<AccountType>().HasData(
                new AccountType
                {
                    Id = Guid.Parse("fa76e2d9-63a2-49f4-8bf8-ca6011e69203"),
                    Type = "Cash"
                },
                new AccountType
                {
                    Id = Guid.Parse("a3eb2441-3d98-4ba1-8955-b051cf8fd94c"),
                    Type = "Card"
                }
            );
        }
    }
}
