using System.ComponentModel.DataAnnotations;

namespace HouseHub.Contracts
{
    public class CreateTodosRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; } = 3; // 1 = High, 2 = Medium-High, 3 = Medium, 4 = Medium-Low, 5 = Low

        [StringLength(50)]
        public string? Category { get; set; }

        public List<Guid>? AssignedUserIds { get; set; } = new List<Guid>();
    }
}