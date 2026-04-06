using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<DoctorSpecialization> DoctorSpecializations => Set<DoctorSpecialization>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Specialization>().HasData(
            new Specialization { Id = 1, Name = "Cardiology", Description = "Heart and cardiovascular system" },
            new Specialization { Id = 2, Name = "Dermatology", Description = "Skin conditions" },
            new Specialization { Id = 3, Name = "Neurology", Description = "Nervous system disorders" },
            new Specialization { Id = 4, Name = "Orthopedics", Description = "Musculoskeletal system" },
            new Specialization { Id = 5, Name = "Pediatrics", Description = "Children's health" },
            new Specialization { Id = 6, Name = "General Medicine", Description = "General health care" }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
