using System.ComponentModel.DataAnnotations;

namespace Case_1_2.Core.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }
        
        public bool IsRevoked { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
