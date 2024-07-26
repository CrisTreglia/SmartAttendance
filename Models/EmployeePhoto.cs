using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendanceSystem.Models
{
    [Table("EmployeePhoto")]
    public class EmployeePhoto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
