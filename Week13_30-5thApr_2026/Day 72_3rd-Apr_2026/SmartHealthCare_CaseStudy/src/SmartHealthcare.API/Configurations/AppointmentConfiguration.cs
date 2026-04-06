using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Status).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Prescription)
               .WithOne(p => p.Appointment)
               .HasForeignKey<Prescription>(p => p.AppointmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
