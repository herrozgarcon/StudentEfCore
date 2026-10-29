using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

namespace StudentCourseEFCore.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService(DepartmentRepository repository)
    {
        _repository = repository;
    }

    public Task<int> AddAsync(Department department)
        => _repository.AddAsync(department);

    public Task<List<Department>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Department?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public Task<bool> UpdateAsync(
        int id, string name, string? location)
        => _repository.UpdateAsync(id, name, location);

    public Task<bool> DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}