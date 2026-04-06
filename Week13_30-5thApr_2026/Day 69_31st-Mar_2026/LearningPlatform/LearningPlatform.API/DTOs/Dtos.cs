using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.API.DTOs
{
    // ==========================================
    // AUTH DTOs
    // ==========================================
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "Student";
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    // ==========================================
    // USER DTOs
    // ==========================================
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ProfileDto? Profile { get; set; }
    }

    public class ProfileDto
    {
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Website { get; set; }
    }

    public class UpdateProfileDto
    {
        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }
    }

    // ==========================================
    // COURSE DTOs
    // ==========================================
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int LessonCount { get; set; }
        public int EnrollmentCount { get; set; }
        public List<LessonDto> Lessons { get; set; } = new();
    }

    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
        public decimal Price { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool IsPublished { get; set; } = false;
    }

    public class UpdateCourseDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Range(0, 10000)]
        public decimal? Price { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool? IsPublished { get; set; }
    }

    // ==========================================
    // LESSON DTOs
    // ==========================================
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; }
    }

    public class CreateLessonDto
    {
        [Required(ErrorMessage = "Lesson title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Content { get; set; }

        public string? VideoUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int OrderIndex { get; set; }

        [Range(0, 600)]
        public int DurationMinutes { get; set; }
    }

    // ==========================================
    // ENROLLMENT DTOs
    // ==========================================
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateEnrollmentDto
    {
        [Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }
    }

    // ==========================================
    // ERROR DTO
    // ==========================================
    public class ErrorResponseDto
    {
        public string Error { get; set; } = string.Empty;
        public List<string>? Details { get; set; }
    }
}
