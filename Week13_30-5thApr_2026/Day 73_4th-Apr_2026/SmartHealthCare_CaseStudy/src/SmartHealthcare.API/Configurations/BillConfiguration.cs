using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ConsultationFee)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(b => b.MedicineCharges)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(b => b.TotalAmount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(b => b.PaymentStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasCheckConstraint("CK_Bill_PaymentStatus",
            "[PaymentStatus] IN ('Paid', 'Unpaid', 'Waived')");

        builder.Property(b => b.GeneratedAt)
            .IsRequired();

        builder.Property(b => b.PaidAt)
            .IsRequired(false);

        builder.Property(b => b.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(b => b.Appointment)
            .WithOne(a => a.Bill)
            .HasForeignKey<Bill>(b => b.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.AppointmentId)
            .IsUnique();

        builder.ToTable("Bills");
    }
}