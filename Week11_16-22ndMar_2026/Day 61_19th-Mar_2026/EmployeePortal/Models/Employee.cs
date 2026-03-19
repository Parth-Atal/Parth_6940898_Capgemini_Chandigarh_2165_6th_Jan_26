using System.ComponentModel.DataAnnotations;

public class Employee
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Department { get; set; }

    [Range(1, double.MaxValue)]
    public decimal Salary { get; set; }
}