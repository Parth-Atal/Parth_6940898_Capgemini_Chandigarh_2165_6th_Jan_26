namespace LearningPlatform.API.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        // Progress percentage 0-100
        public int Progress { get; set; } = 0;

        // Foreign keys (Many-to-Many: User <-> Course)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
