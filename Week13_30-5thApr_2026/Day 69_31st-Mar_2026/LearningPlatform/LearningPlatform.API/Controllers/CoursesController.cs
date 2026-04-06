using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace LearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CoursesCacheKey = "all_courses";
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public CoursesController(AppDbContext context, IMapper mapper, IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        {
            // Check cache first
            if (_cache.TryGetValue(CoursesCacheKey, out List<CourseDto>? cachedCourses))
            {
                return Ok(new { source = "cache", data = cachedCourses });
            }

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            // Store in cache for 5 minutes
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _cache.Set(CoursesCacheKey, courseDtos, cacheOptions);

            return Ok(new { source = "database", data = courseDtos });
        }

        
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons.OrderBy(l => l.OrderIndex))
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound(new ErrorResponseDto { Error = $"Course with ID {id} not found" });

            var courseDto = _mapper.Map<CourseDto>(course);
            return Ok(courseDto);
        }

       
        [HttpGet("category/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoursesByCategory(string name)
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Where(c => c.Category.ToLower() == name.ToLower() && c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);
            return Ok(courseDtos);
        }

        
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
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

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token"));

            var course = _mapper.Map<Course>(dto);
            course.InstructorId = userId;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Invalidate cache
            _cache.Remove(CoursesCacheKey);

            var createdCourse = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == course.Id);

            var courseDto = _mapper.Map<CourseDto>(createdCourse!);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, courseDto);
        }

        
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new ErrorResponseDto { Error = $"Course with ID {id} not found" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only instructor who created the course or Admin can update
            if (course.InstructorId != userId && userRole != "Admin")
                return Forbid();

            _mapper.Map(dto, course);
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Invalidate cache
            _cache.Remove(CoursesCacheKey);

            return Ok(new { message = "Course updated successfully" });
        }

        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new ErrorResponseDto { Error = $"Course with ID {id} not found" });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            // Invalidate cache
            _cache.Remove(CoursesCacheKey);

            return Ok(new { message = "Course deleted successfully" });
        }

        
        [HttpPost("{id:int}/lessons")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> AddLesson(int id, [FromBody] CreateLessonDto dto)
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

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new ErrorResponseDto { Error = $"Course with ID {id} not found" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (course.InstructorId != userId && userRole != "Admin")
                return Forbid();

            var lesson = _mapper.Map<Lesson>(dto);
            lesson.CourseId = id;

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            var lessonDto = _mapper.Map<LessonDto>(lesson);
            return CreatedAtAction(nameof(GetCourseById), new { id }, lessonDto);
        }

        
        [HttpGet("my")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Where(c => c.InstructorId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);
            return Ok(courseDtos);
        }
    }
}
