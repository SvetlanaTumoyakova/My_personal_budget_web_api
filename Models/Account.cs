namespace My_personal_budget_web_api.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Название счёта (например, «Основная карта», «Наличные»)
        public AccountType Type { get; set; } // Тип счёта: наличные или карта
        public double Balance { get; set; } = 0.00; // Баланс
        public bool IsDefault { get; set; } = false; // счёт по умолчанию (может быть только 1)
        public bool IsDeleted { get; set; } = false; // Логическое удаление
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; } 

    }
    public enum AccountType
    {
        Cash = 1,
        Card = 2
    }
}
