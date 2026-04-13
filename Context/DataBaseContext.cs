using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Context
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
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

            // Уникальный индекс для счёта по умолчанию у пользователя
            modelBuilder.Entity<Account>()
                .HasIndex(a => new { a.UserId, a.IsDefault })
                .HasFilter("\"IsDefault\" = TRUE")
                .IsUnique();
        }
    }
}
