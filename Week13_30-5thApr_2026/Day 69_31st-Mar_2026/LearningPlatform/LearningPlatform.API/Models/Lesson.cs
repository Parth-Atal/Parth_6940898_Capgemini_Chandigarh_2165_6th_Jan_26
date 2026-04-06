using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.API.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Content { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        public int OrderIndex { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key - Many lessons belong to one Course
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
