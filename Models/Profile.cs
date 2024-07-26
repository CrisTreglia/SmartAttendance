using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendanceSystem.Models
{
    [Table("Profile")]
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Don't auto-increment
        public int ProfileID { get; set; }

        [Required]
        [StringLength(10)] // Adjust if label gets longer
        public string ProfileName { get; set; }
    }
}
