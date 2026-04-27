using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My_personal_budget_web_api.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public decimal Amount { get; set; } // Сумма транзакции

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow; // Дата транзакции

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        [Required]
        public Guid TransactionTypeId { get; set; }
        public TransactionType? TransactionType { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания записи
        public DateTime? UpdatedAt { get; set; }
    }
}
