
namespace StudentCourseEFCore.Models;

public class Student
{
    public int StudentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }

    // Foreign key
    public int DepartmentId { get; set; }

    // Navigation property
    public Department? Department { get; set; }

    // Navigation property for enrollments
    public ICollection<Enrollment> Enrollments { get; set; }
       = new List<Enrollment>();
}
