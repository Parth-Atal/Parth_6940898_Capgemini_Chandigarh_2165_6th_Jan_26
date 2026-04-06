using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Diagnosis).IsRequired().HasMaxLength(500);

        builder.HasMany(p => p.Medicines)
               .WithOne(m => m.Prescription)
               .HasForeignKey(m => m.PrescriptionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
