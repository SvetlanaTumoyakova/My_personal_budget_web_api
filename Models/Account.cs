using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My_personal_budget_web_api.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Название счёта (например, «Основная карта», «Наличные»)

        [Required]
        public Guid AccountTypeId { get; set; } // Тип счёта: наличные или карта
        public AccountType AccountType { get; set; } = null!;

        public decimal Balance { get; set; } = 0.00m; // Баланс

        public bool IsDefault { get; set; } = false; // счёт по умолчанию (может быть только 1)
       
        public bool IsDeleted { get; set; } = false; // Логическое удаление

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
