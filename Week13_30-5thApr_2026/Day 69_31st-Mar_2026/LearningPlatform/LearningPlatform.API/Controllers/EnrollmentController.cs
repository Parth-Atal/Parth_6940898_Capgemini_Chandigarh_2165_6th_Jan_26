using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/v1/enroll")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EnrollmentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> EnrollCourse([FromBody] CreateEnrollmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Validation failed",
                    Details = errors
                });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Check if course exists and is published
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return NotFound(new ErrorResponseDto { Error = $"Course with ID {dto.CourseId} not found" });

            if (!course.IsPublished)
                return BadRequest(new ErrorResponseDto { Error = "Course is not available for enrollment" });

            // Check if already enrolled
            var existing = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == dto.CourseId);

            if (existing)
                return Conflict(new ErrorResponseDto { Error = "You are already enrolled in this course" });

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = dto.CourseId,
                EnrolledAt = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully enrolled in course", courseTitle = course.Title });
        }

        
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);
            return Ok(enrollmentDtos);
        }

        
        [HttpDelete("{courseId:int}")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound(new ErrorResponseDto { Error = "Enrollment not found" });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unenrolled from course" });
        }

        
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            var result = enrollments.Select(e => new
            {
                e.Id,
                Student = e.User.Username,
                StudentEmail = e.User.Email,
                Course = e.Course.Title,
                Instructor = e.Course.Instructor.Username,
                e.EnrolledAt,
                e.Progress,
                e.IsCompleted
            });

            return Ok(result);
        }
    }
}
