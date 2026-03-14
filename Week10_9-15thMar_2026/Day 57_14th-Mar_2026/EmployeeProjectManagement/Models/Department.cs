using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectManagement.Models
{
    public class Department
    {

        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}