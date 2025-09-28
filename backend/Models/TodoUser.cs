using System.ComponentModel.DataAnnotations;

namespace HouseHub.Models
{
    public class TodoUser
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TodoId { get; set; }
        public Todo Todo { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}