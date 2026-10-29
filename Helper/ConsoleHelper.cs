using StudentCourseEFCore.Models;

namespace StudentCourseEFCore.Helpers;

public static class ConsoleHelper
{
    public static string ReadRequiredInput(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    public static bool TryReadId(string message, out int id)
    {
        Console.Write(message);

        if (int.TryParse(Console.ReadLine(), out id) && id > 0)
            return true;

        Console.WriteLine("Enter a valid positive ID.");
        return false;
    }

    public static bool TryReadAge(out int age)
    {
        Console.Write("Enter student age: ");

        if (int.TryParse(Console.ReadLine(), out age)
            && age >= 17 && age <= 60)
            return true;

        Console.WriteLine("Age must be between 17 and 60.");
        return false;
    }

    public static void DisplayDepartments(List<Department> departments)
    {
        Console.WriteLine("\nAvailable Departments");
        Console.WriteLine("ID | Department Name");

        foreach (var department in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} | {department.DepartmentName}");
        }
    }

    public static void DisplayStudents(List<Student> students)
    {
        Console.WriteLine("\nID | Name | Email | Age | Department");

        foreach (var student in students)
        {
            Console.WriteLine(
                $"{student.StudentId} | " +
                $"{student.Name} | " +
                $"{student.Email} | " +
                $"{student.Age} | " +
                $"{student.Department?.DepartmentName ?? "Not loaded"}");
        }
    }

    public static void DisplayStudent(Student student)
    {
        Console.WriteLine("\nStudent Details");
        Console.WriteLine($"ID: {student.StudentId}");
        Console.WriteLine($"Name: {student.Name}");
        Console.WriteLine($"Email: {student.Email}");
        Console.WriteLine($"Age: {student.Age}");
        Console.WriteLine($"Department ID: {student.DepartmentId}");
        Console.WriteLine(
            $"Department: {student.Department?.DepartmentName ?? "Not loaded"}");
    }
}