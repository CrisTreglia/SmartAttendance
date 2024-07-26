using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAttendanceSystem.Models
{
    [Table("Attendance")]
    public class Attendance
    {
        [Key] // Marks the property as the primary key
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Column(TypeName = "datetime")] // Stores only the date part (without time)
        public DateTime AttendanceDate { get; set; }
    }
}
