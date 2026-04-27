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

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Category> Categories { get; set; }

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

            // Настройка связи один-ко-многим: TransactionType → Transactions
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TransactionType)
                .WithMany(tt => tt.Transactions)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи один-ко-многим: Category → Transactions
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи один-ко-многим: Account → Transactions
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи один-ко-многим: User → Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Заполнение данных для TransactionType
            modelBuilder.Entity<TransactionType>().HasData(
                new TransactionType
                {
                    Id = Guid.Parse("b2bc0dea-11a3-4810-b45f-d0fce65b9004"),
                    Type = "Income"
                },
                new TransactionType
                {
                    Id = Guid.Parse("0219dfdb-fdae-4a26-b5d4-bdc74441e266"),
                    Type = "Expense"
                },
                new TransactionType
                {
                    Id = Guid.Parse("aed1c283-4ee4-4ece-924d-9d44c0d389d5"),
                    Type = "Transfer" // для переводов между счетами
                }
            );

            // Заполнение данных для Categories (по умолчанию)
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = Guid.Parse("e5f730a5-a546-4b74-adb7-10d4258e1c98"),
                    Name = "Зарплата",
                    IsDefault = true,
                    TransactionTypeId = Guid.Parse("b2bc0dea-11a3-4810-b45f-d0fce65b9004"), // Income
                    UserId = Guid.Empty
                },
                 new Category
                 {
                     Id = Guid.Parse("7f120d3f-b685-4b9c-ae77-337ca22e5012"),
                     Name = "Продукты",
                     IsDefault = true,
                     TransactionTypeId = Guid.Parse("0219dfdb-fdae-4a26-b5d4-bdc74441e266"), // Expense
                     UserId = Guid.Empty
                 },
                new Category
                {
                    Id = Guid.Parse("d7e39456-bf47-402c-a92c-f000b73c55cc"),
                    Name = "Транспорт",
                    IsDefault = true,
                    TransactionTypeId = Guid.Parse("0219dfdb-fdae-4a26-b5d4-bdc74441e266"), // Expense
                    UserId = Guid.Empty
                }
            );
        }
    }
}
