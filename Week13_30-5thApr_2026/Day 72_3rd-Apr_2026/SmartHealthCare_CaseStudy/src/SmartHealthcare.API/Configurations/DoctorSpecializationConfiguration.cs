using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class DoctorSpecializationConfiguration : IEntityTypeConfiguration<DoctorSpecialization>
{
    public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
    {
        builder.HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

        builder.HasOne(ds => ds.Doctor)
               .WithMany(d => d.DoctorSpecializations)
               .HasForeignKey(ds => ds.DoctorId);

        builder.HasOne(ds => ds.Specialization)
               .WithMany(s => s.DoctorSpecializations)
               .HasForeignKey(ds => ds.SpecializationId);
    }
}
