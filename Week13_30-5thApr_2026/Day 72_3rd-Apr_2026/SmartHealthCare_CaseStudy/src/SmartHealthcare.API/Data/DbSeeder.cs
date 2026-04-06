using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await SeedDoctorsAsync(db);
        await SeedPatientsAsync(db);
        await SeedAppointmentsAsync(db);
        await SeedPrescriptionsAsync(db);
    }

    private static async Task SeedDoctorsAsync(ApplicationDbContext db)
    {
        if (await db.Doctors.AnyAsync())
        {
            return;
        }

        var specializationIds = await db.Specializations
            .Select(s => s.Id)
            .ToListAsync();

        if (specializationIds.Count == 0)
        {
            return;
        }

        var doctorUsers = new List<User>
        {
            new()
            {
                FullName = "Dr. Farrukh Tashkentov",
                Email = "ftashkentov@smarthealthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Leilani Nakamura",
                Email = "lnakamura@smarthealthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Obinna Eze",
                Email = "oeze@smarthealthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Catalina Florescu",
                Email = "cflorescu@smarthealthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Haruto Yamashita",
                Email = "hyamashita@smarthealthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Users.AddRangeAsync(doctorUsers);
        await db.SaveChangesAsync();

        var doctors = new List<Doctor>
        {
            new()
            {
                UserId = doctorUsers[0].Id,
                LicenseNumber = "LIC-NEURO-2201",
                YearsOfExperience = 14,
                ConsultationFee = 1100,
                Phone = "8800001111",
                IsAvailable = true
            },
            new()
            {
                UserId = doctorUsers[1].Id,
                LicenseNumber = "LIC-PEDI-2202",
                YearsOfExperience = 8,
                ConsultationFee = 750,
                Phone = "8800002222",
                IsAvailable = true
            },
            new()
            {
                UserId = doctorUsers[2].Id,
                LicenseNumber = "LIC-ORTH-2203",
                YearsOfExperience = 5,
                ConsultationFee = 500,
                Phone = "8800003333",
                IsAvailable = false
            },
            new()
            {
                UserId = doctorUsers[3].Id,
                LicenseNumber = "LIC-DERM-2204",
                YearsOfExperience = 10,
                ConsultationFee = 850,
                Phone = "8800004444",
                IsAvailable = true
            },
            new()
            {
                UserId = doctorUsers[4].Id,
                LicenseNumber = "LIC-CARD-2205",
                YearsOfExperience = 17,
                ConsultationFee = 1300,
                Phone = "8800005555",
                IsAvailable = true
            }
        };

        await db.Doctors.AddRangeAsync(doctors);
        await db.SaveChangesAsync();

        var doctorSpecializations = new List<DoctorSpecialization>
        {
            new() { DoctorId = doctors[0].Id, SpecializationId = 3 }, // Neurology
            new() { DoctorId = doctors[0].Id, SpecializationId = 6 }, // General Medicine
            new() { DoctorId = doctors[1].Id, SpecializationId = 5 }, // Pediatrics
            new() { DoctorId = doctors[2].Id, SpecializationId = 4 }, // Orthopedics
            new() { DoctorId = doctors[3].Id, SpecializationId = 2 }, // Dermatology
            new() { DoctorId = doctors[4].Id, SpecializationId = 1 }  // Cardiology
        };

        await db.DoctorSpecializations.AddRangeAsync(doctorSpecializations);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPatientsAsync(ApplicationDbContext db)
    {
        if (await db.Patients.AnyAsync())
        {
            return;
        }

        var patientUsers = new List<User>
        {
            new()
            {
                FullName = "Miriam Okonkwo",
                Email = "mokonkwo@mailhub.net",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Tomasz Wierzbicki",
                Email = "twierzbicki@mailhub.net",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Yuna Baek",
                Email = "ybaek@mailhub.net",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Darius Blackwood",
                Email = "dblackwood@mailhub.net",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Isabela Ferreira",
                Email = "iferreira@mailhub.net",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Users.AddRangeAsync(patientUsers);
        await db.SaveChangesAsync();

        var patients = new List<Patient>
        {
            new()
            {
                UserId = patientUsers[0].Id,
                DateOfBirth = new DateTime(1978, 2, 11),
                Gender = "Female",
                BloodGroup = "B-",
                Address = "14 Crescent Boulevard, Pune, MH 411001",
                Phone = "7700001111"
            },
            new()
            {
                UserId = patientUsers[1].Id,
                DateOfBirth = new DateTime(1983, 6, 29),
                Gender = "Male",
                BloodGroup = "A-",
                Address = "77 Willowbrook Drive, Kolkata, WB 700001",
                Phone = "7700002222"
            },
            new()
            {
                UserId = patientUsers[2].Id,
                DateOfBirth = new DateTime(2000, 12, 3),
                Gender = "Female",
                BloodGroup = "O+",
                Address = "9 Sunrise Terrace, Ahmedabad, GJ 380001",
                Phone = "7700003333"
            },
            new()
            {
                UserId = patientUsers[3].Id,
                DateOfBirth = new DateTime(1970, 9, 17),
                Gender = "Male",
                BloodGroup = "AB-",
                Address = "52 Harbour View, Kochi, KL 682001",
                Phone = "7700004444"
            },
            new()
            {
                UserId = patientUsers[4].Id,
                DateOfBirth = new DateTime(1997, 4, 25),
                Gender = "Female",
                BloodGroup = "B+",
                Address = "38 Maple Court, Jaipur, RJ 302001",
                Phone = "7700005555"
            }
        };

        await db.Patients.AddRangeAsync(patients);
        await db.SaveChangesAsync();
    }

    private static async Task SeedAppointmentsAsync(ApplicationDbContext db)
    {
        if (await db.Appointments.AnyAsync())
        {
            return;
        }

        var patients = await db.Patients.Take(3).ToListAsync();
        var doctors = await db.Doctors.Take(3).ToListAsync();

        if (patients.Count < 3 || doctors.Count < 3)
        {
            return;
        }

        var appointments = new List<Appointment>
        {
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctors[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(2).AddHours(9),
                Status = "Pending",
                Notes = "Persistent migraines and dizziness for 3 weeks",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctors[2].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(4).AddHours(13),
                Status = "Pending",
                Notes = "Knee swelling after sports injury",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctors[1].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(6).AddHours(16),
                Status = "Pending",
                Notes = "Routine wellness checkup",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctors[2].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(-4).AddHours(11),
                Status = "Completed",
                Notes = "Post-surgery follow-up for wrist fracture",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await db.Appointments.AddRangeAsync(appointments);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPrescriptionsAsync(ApplicationDbContext db)
    {
        if (await db.Prescriptions.AnyAsync())
        {
            return;
        }

        var completedAppointments = await db.Appointments
            .Where(a => a.Status == "Completed")
            .Take(1)
            .ToListAsync();

        if (completedAppointments.Count == 0)
        {
            return;
        }

        var appointment = completedAppointments[0];

        var medicines = new List<Medicine>
        {
            new() { Name = "Ibuprofen", Dosage = "400mg", Duration = "14 days", Instructions = "Twice daily with food" },
            new() { Name = "Calcium Carbonate", Dosage = "500mg", Duration = "90 days", Instructions = "Once daily at bedtime" }
        };

        var prescription = new Prescription
        {
            AppointmentId = appointment.Id,
            Diagnosis = "Post-operative wrist fracture recovery with mild inflammation",
            Notes = "Avoid lifting heavy objects. Physical therapy sessions recommended twice weekly.",
            CreatedAt = DateTime.UtcNow,
            Medicines = medicines
        };

        await db.Prescriptions.AddAsync(prescription);
        await db.SaveChangesAsync();
    }
}