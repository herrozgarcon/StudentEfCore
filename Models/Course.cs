namespace StudentCourseEFCore.Models;

public class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int Credits { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property for enrollments
    public ICollection<Enrollment> Enrollments { get; set; }
      = new List<Enrollment>();
}

