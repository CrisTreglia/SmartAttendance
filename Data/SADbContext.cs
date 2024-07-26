using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartAttendanceSystem.Data
{
    public class SADbContext : DbContext
    {
    public SADbContext(DbContextOptions<SADbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
