using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeProjectManagement.Models
{
    public class EmployeeProject
    {
        [Key]
        public int Id { get; set; } 

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public DateTime AssignedDate { get; set; }
    }
}