using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Models.Entities;

public class Bill
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ConsultationFee { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal MedicineCharges { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Required, MaxLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid";

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

   
    public Appointment Appointment { get; set; } = null!;
}