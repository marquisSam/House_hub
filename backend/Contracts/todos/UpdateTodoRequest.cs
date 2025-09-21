using System.ComponentModel.DataAnnotations;

namespace HouseHub.Contracts
{
    public class UpdateTodoRequest
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int? Priority { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }
    }
}