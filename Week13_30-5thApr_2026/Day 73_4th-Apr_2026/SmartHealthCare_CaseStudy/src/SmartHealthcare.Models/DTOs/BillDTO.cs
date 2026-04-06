using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;


public class BillDTO
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }

    
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;

    
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    
    public DateTime AppointmentDate { get; set; }

    
    public decimal ConsultationFee { get; set; }
    public decimal MedicineCharges { get; set; }
    public decimal TotalAmount { get; set; }

    
    public string PaymentStatus { get; set; } = "Unpaid";
    public DateTime GeneratedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }

    
    public List<BillMedicineLineDTO> MedicineLines { get; set; } = new();
}


public class BillMedicineLineDTO
{
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public decimal Charge { get; set; }
}


public class GenerateBillDTO
{
    [Required(ErrorMessage = "Appointment ID is required")]
    public int AppointmentId { get; set; }

    
    [Range(0, 100000, ErrorMessage = "Medicine charges must be between 0 and 100,000")]
    public decimal? MedicineChargesOverride { get; set; }

    
    [Range(0, 10000, ErrorMessage = "Per medicine charge must be between 0 and 10,000")]
    public decimal? PerMedicineCharge { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}


public class UpdateBillDTO
{
    [Required(ErrorMessage = "Payment status is required")]
    [RegularExpression("^(Paid|Unpaid|Waived)$",
        ErrorMessage = "Payment status must be Paid, Unpaid, or Waived")]
    public string PaymentStatus { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}