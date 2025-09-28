using System.ComponentModel.DataAnnotations;

namespace HouseHub.Models
{
    public class Todo
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; } = 3; // 1 = High, 2 = Medium-High, 3 = Medium, 4 = Medium-Low, 5 = Low

        [MaxLength(50)]
        public string? Category { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for many-to-many relationship
        public ICollection<TodoUser> TodoUsers { get; set; } = new List<TodoUser>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}