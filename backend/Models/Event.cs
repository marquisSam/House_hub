using System.ComponentModel.DataAnnotations;

namespace HouseHub.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [Required]
        public bool IsAllDay { get; set; } = false;

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(7)] // For hex color codes like #FF5733
        public string? Color { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Recurrence properties
        public bool IsRecurring { get; set; } = false;

        [MaxLength(50)]
        public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly, Yearly

        public DateTime? RecurrenceEndDate { get; set; }

        // Reminder properties
        public bool HasReminder { get; set; } = false;

        public int? ReminderMinutesBefore { get; set; }

        // Priority level
        [Range(1, 5)]
        public int Priority { get; set; } = 3; // 1 = High, 5 = Low
    }
}