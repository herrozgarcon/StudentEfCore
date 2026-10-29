using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

namespace StudentCourseEFCore.Services;

public class StudentService
{
    private readonly StudentRepository _repository;

    public StudentService(StudentRepository repository)
    {
        _repository = repository;
    }

    public Task<int> AddAsync(Student student)
        => _repository.AddAsync(student);

    public Task<List<Student>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Student?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public Task<bool> UpdateAsync(
        int id,
        string name,
        string email,
        int age,
        int departmentId)
        => _repository.UpdateAsync(
            id, name, email, age, departmentId);

    public Task<bool> DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}