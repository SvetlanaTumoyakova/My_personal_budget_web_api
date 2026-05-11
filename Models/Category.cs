using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My_personal_budget_web_api.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false; // Для предустановленных категорий
        public bool IsDeleted { get; set; } = false; // Логическое удаление

        [Required]
        public Guid TransactionTypeId { get; set; }
        [Required]
        public TransactionType? TransactionType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
