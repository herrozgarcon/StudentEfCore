using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

namespace StudentCourseEFCore.Services;

public class EnrollmentService
{
    private readonly EnrollmentRepository _repository;

    public EnrollmentService(EnrollmentRepository repository)
    {
        _repository = repository;
    }

    public Task<int> AddAsync(Enrollment enrollment)
    {
        return _repository.AddAsync(enrollment);
    }

    public Task<List<Enrollment>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Enrollment?> GetByIdAsync(int enrollmentId)
    {
        return _repository.GetByIdAsync(enrollmentId);
    }

    public Task<List<Enrollment>> GetByStudentIdAsync(int studentId)
    {
        return _repository.GetByStudentIdAsync(studentId);
    }

    public Task<bool> DeleteAsync(int enrollmentId)
    {
        return _repository.DeleteAsync(enrollmentId);
    }
}