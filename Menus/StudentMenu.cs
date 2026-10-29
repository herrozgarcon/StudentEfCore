
using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Helpers;
using StudentCourseEFCore.Models;
using StudentCourseEFCore.Services;

namespace StudentCourseEFCore.Menus;

public class StudentMenu
{
    private readonly StudentService _studentService;
    private readonly DepartmentService _departmentService;

    public StudentMenu(
        StudentService studentService,
        DepartmentService departmentService)
    {
        _studentService = studentService;
        _departmentService = departmentService;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===== STUDENT MANAGEMENT =====");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. View Student By ID");
            Console.WriteLine("4. Update Student");
            Console.WriteLine("5. Delete Student");
            Console.WriteLine("0. Back to Main Menu");

            Console.Write("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    await AddAsync();
                    break;

                case 2:
                    await GetAllAsync();
                    break;

                case 3:
                    await GetByIdAsync();
                    break;

                case 4:
                    await UpdateAsync();
                    break;

                case 5:
                    await DeleteAsync();
                    break;

                case 0:
                    return;

                default:
                    Console.WriteLine(
                        "Invalid choice. Select an option from 0 to 5.");
                    break;
            }
        }
    }

    private async Task AddAsync()
    {
        var departments = await _departmentService.GetAllAsync();

        if (departments.Count == 0)
        {
            Console.WriteLine(
                "Create a department before adding students.");
            return;
        }

        ConsoleHelper.DisplayDepartments(departments);

        string name = ConsoleHelper.ReadRequiredInput(
            "Enter student name: ");

        string email = ConsoleHelper.ReadRequiredInput(
            "Enter student email: ");

        if (!ConsoleHelper.TryReadAge(out int age))
            return;

        if (!ConsoleHelper.TryReadId(
                "Enter department ID: ", out int departmentId))
            return;

        if (!departments.Any(d => d.DepartmentId == departmentId))
        {
            Console.WriteLine("Department not found.");
            return;
        }

        var student = new Student
        {
            Name = name,
            Email = email,
            Age = age,
            DepartmentId = departmentId
        };

        try
        {
            int id = await _studentService.AddAsync(student);

            Console.WriteLine(
                $"Student added successfully. ID: {id}");
        }
        catch (DbUpdateException)
        {
            Console.WriteLine(
                "Could not add student. Check the email " +
                "and database constraints.");
        }
    }

    private async Task GetAllAsync()
    {
        var students = await _studentService.GetAllAsync();

        if (students.Count == 0)
        {
            Console.WriteLine("No students found.");
            return;
        }

        ConsoleHelper.DisplayStudents(students);
    }

    private async Task GetByIdAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter student ID: ", out int id))
            return;

        var student = await _studentService.GetByIdAsync(id);

        if (student is null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        ConsoleHelper.DisplayStudent(student);
    }

    private async Task UpdateAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter student ID to update: ", out int id))
            return;

        var existingStudent = await _studentService.GetByIdAsync(id);

        if (existingStudent is null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        var departments = await _departmentService.GetAllAsync();

        if (departments.Count == 0)
        {
            Console.WriteLine("No departments available.");
            return;
        }

        ConsoleHelper.DisplayDepartments(departments);

        string name = ConsoleHelper.ReadRequiredInput(
            "Enter new student name: ");

        string email = ConsoleHelper.ReadRequiredInput(
            "Enter new student email: ");

        if (!ConsoleHelper.TryReadAge(out int age))
            return;

        if (!ConsoleHelper.TryReadId(
                "Enter new department ID: ", out int departmentId))
            return;

        if (!departments.Any(d => d.DepartmentId == departmentId))
        {
            Console.WriteLine("Department not found.");
            return;
        }

        try
        {
            bool updated = await _studentService.UpdateAsync(
                id, name, email, age, departmentId);

            Console.WriteLine(
                updated
                    ? "Student updated successfully."
                    : "Student not found.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (DbUpdateException)
        {
            Console.WriteLine(
                "Could not update student. Check the email " +
                "and database constraints.");
        }
    }

    private async Task DeleteAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter student ID to delete: ", out int id))
            return;

        var student = await _studentService.GetByIdAsync(id);

        if (student is null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.Write(
            $"Delete student '{student.Name}'? (y/n): ");

        if (!string.Equals(
                Console.ReadLine(),
                "y",
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        bool deleted = await _studentService.DeleteAsync(id);

        Console.WriteLine(
            deleted
                ? "Student deleted successfully."
                : "Student not found.");
    }
}
