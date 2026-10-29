namespace StudentCourseEFCore.Models;

public class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    // Collection navigation property
    public ICollection<Student> Students { get; set; }
        = new List<Student>();
}

