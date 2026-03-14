using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Many-to-Many Navigation
        public ICollection<EmployeeProject>? EmployeeProjects { get; set; }
    }
}