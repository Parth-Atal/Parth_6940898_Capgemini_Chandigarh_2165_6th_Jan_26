using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Gender).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Phone).IsRequired().HasMaxLength(15);
        builder.Property(p => p.BloodGroup).HasMaxLength(5);
    }
}
