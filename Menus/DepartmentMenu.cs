using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Helpers;
using StudentCourseEFCore.Models;
using StudentCourseEFCore.Services;

namespace StudentCourseEFCore.Menus;

public class DepartmentMenu
{
    private readonly DepartmentService _service;

    public DepartmentMenu(DepartmentService service)
    {
        _service = service;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===== DEPARTMENT MANAGEMENT =====");
            Console.WriteLine("1. Add Department");
            Console.WriteLine("2. View All Departments");
            Console.WriteLine("3. View Department By ID");
            Console.WriteLine("4. Update Department");
            Console.WriteLine("5. Delete Department");
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
        string name = ConsoleHelper.ReadRequiredInput(
            "Enter department name: ");

        Console.Write("Enter location: ");
        string? location = Console.ReadLine();

        var department = new Department
        {
            DepartmentName = name,
            Location = string.IsNullOrWhiteSpace(location)
                ? null
                : location.Trim()
        };

        try
        {
            int id = await _service.AddAsync(department);

            Console.WriteLine(
                $"Department added successfully. ID: {id}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (DbUpdateException)
        {
            Console.WriteLine(
                "A database error occurred while adding the department.");
        }
    }

    private async Task GetAllAsync()
    {
        var departments = await _service.GetAllAsync();

        if (departments.Count == 0)
        {
            Console.WriteLine("No departments found.");
            return;
        }

        Console.WriteLine(
            "\nID | Department Name | Location | Active");

        foreach (var department in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} | " +
                $"{department.DepartmentName} | " +
                $"{department.Location} | " +
                $"{department.IsActive}");
        }
    }

    private async Task GetByIdAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter department ID: ", out int id))
        {
            return;
        }

        var department = await _service.GetByIdAsync(id);

        if (department is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        Console.WriteLine(
            $"{department.DepartmentId} | " +
            $"{department.DepartmentName} | " +
            $"{department.Location} | " +
            $"{department.IsActive}");
    }

    private async Task UpdateAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter department ID to update: ", out int id))
        {
            return;
        }

        string name = ConsoleHelper.ReadRequiredInput(
            "Enter new department name: ");

        Console.Write("Enter new location: ");
        string? location = Console.ReadLine();

        location = string.IsNullOrWhiteSpace(location)
            ? null
            : location.Trim();

        try
        {
            bool updated = await _service.UpdateAsync(
                id, name, location);

            Console.WriteLine(
                updated
                    ? "Department updated successfully."
                    : "Department not found.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (DbUpdateException)
        {
            Console.WriteLine(
                "A database error occurred while updating the department.");
        }
    }

    private async Task DeleteAsync()
    {
        if (!ConsoleHelper.TryReadId(
                "Enter department ID to delete: ", out int id))
        {
            return;
        }

        var department = await _service.GetByIdAsync(id);

        if (department is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        Console.Write(
            $"Delete department '{department.DepartmentName}'? (y/n): ");

        if (!string.Equals(
                Console.ReadLine(),
                "y",
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        try
        {
            bool deleted = await _service.DeleteAsync(id);

            Console.WriteLine(
                deleted
                    ? "Department deleted successfully."
                    : "Department not found.");
        }
        catch (DbUpdateException)
        {
            Console.WriteLine(
                "Cannot delete this department. " +
                "It may have students assigned to it.");
        }
    }
}