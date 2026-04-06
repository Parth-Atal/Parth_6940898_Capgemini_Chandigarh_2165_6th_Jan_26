using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(d => d.ConsultationFee).HasColumnType("decimal(10,2)");
        builder.Property(d => d.Phone).IsRequired().HasMaxLength(15);
    }
}
