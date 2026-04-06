using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.API.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key - Instructor who created the course
        public int InstructorId { get; set; }
        public User Instructor { get; set; } = null!;

        // One-to-Many: Course has many Lessons
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        // Many-to-Many: Course enrolled by many Students
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
