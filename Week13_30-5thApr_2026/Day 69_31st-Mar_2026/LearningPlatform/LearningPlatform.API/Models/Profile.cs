using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.API.Models
{
    public class Profile
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key for one-to-one with User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
