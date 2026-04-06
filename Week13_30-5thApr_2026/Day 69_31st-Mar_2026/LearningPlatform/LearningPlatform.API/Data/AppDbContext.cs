using LearningPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // ONE-TO-ONE: User <-> Profile
            // ==========================================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // ONE-TO-MANY: User (Instructor) -> Courses
            // ==========================================
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade to avoid conflict

            // ==========================================
            // ONE-TO-MANY: Course -> Lessons
            // ==========================================
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // MANY-TO-MANY: User <-> Course (via Enrollment)
            // ==========================================
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: a user can only enroll once per course
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();

            // ==========================================
            // COLUMN CONFIGURATIONS
            // ==========================================
            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ==========================================
            // SEED DATA
            // ==========================================
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@learnify.com",
                    // Password: Admin@123
                    PasswordHash = "$2a$11$rBnOFCbSGxBdKI9FPrZBHuT.JfNaVRKO7LNL1q5lCMxUJTElQwkAq",
                    Role = "Admin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Username = "instructor1",
                    Email = "instructor@learnify.com",
                    // Password: Instructor@123
                    PasswordHash = "$2a$11$rBnOFCbSGxBdKI9FPrZBHuT.JfNaVRKO7LNL1q5lCMxUJTElQwkAq",
                    Role = "Instructor",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "ASP.NET Core Web API Masterclass",
                    Description = "Learn to build production-ready APIs with .NET 9",
                    Category = "Web Development",
                    Price = 49.99m,
                    IsPublished = true,
                    InstructorId = 2,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Course
                {
                    Id = 2,
                    Title = "React & TypeScript Fundamentals",
                    Description = "Build modern UIs with React and TypeScript",
                    Category = "Frontend",
                    Price = 39.99m,
                    IsPublished = true,
                    InstructorId = 2,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
