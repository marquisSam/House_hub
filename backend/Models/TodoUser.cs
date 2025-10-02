using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HouseHub.Models
{
    public class TodoUser
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TodoId { get; set; }
        
        [JsonIgnore] // Prevent circular reference
        public Todo Todo { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        
        [JsonIgnore] // Prevent circular reference  
        public User User { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}